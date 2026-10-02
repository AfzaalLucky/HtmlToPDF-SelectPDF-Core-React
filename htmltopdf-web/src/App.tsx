import { useEffect, useState } from 'react'

// The policy HTML is built server-side with a StringBuilder (see htmltopdf-api/PolicyHtmlBuilder.cs).
const POLICY_HTML_URL = '/api/policy-builder.html'
const POLICY_PDF_URL = '/api/policy-builder.pdf'
const PDF_FILE_NAME = 'AI-Foundry-Policy-AIF-POL-GOV-001.pdf'

type View = 'preview' | 'download'
type PreviewState = { status: 'loading' } | { status: 'ready'; html: string } | { status: 'error'; message: string }
type DownloadState = { status: 'idle' } | { status: 'busy' } | { status: 'done' } | { status: 'error'; message: string }

function saveBlob(blob: Blob, fileName: string) {
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = fileName
  document.body.appendChild(link)
  link.click()
  link.remove()
  // The browser reads the blob URL asynchronously; revoking it immediately can yield an empty/corrupt file.
  setTimeout(() => URL.revokeObjectURL(url), 60_000)
}

function App() {
  const [view, setView] = useState<View>('preview')
  const [download, setDownload] = useState<DownloadState>({ status: 'idle' })
  const [preview, setPreview] = useState<PreviewState>({ status: 'loading' })

  // Fetch the HTML up front so a missing or stale API shows an error instead of an empty frame.
  useEffect(() => {
    const controller = new AbortController()
    fetch(POLICY_HTML_URL, { signal: controller.signal })
      .then(async (response) => {
        if (!response.ok) throw new Error(`Server responded ${response.status} ${response.statusText}`)
        setPreview({ status: 'ready', html: await response.text() })
      })
      .catch((error: unknown) => {
        if (controller.signal.aborted) return
        setPreview({ status: 'error', message: error instanceof Error ? error.message : String(error) })
      })
    return () => controller.abort()
  }, [])

  async function downloadPdf() {
    setView('download')
    setDownload({ status: 'busy' })
    try {
      const response = await fetch(POLICY_PDF_URL)
      if (!response.ok) throw new Error(`Server responded ${response.status} ${response.statusText}`)
      saveBlob(new Blob([await response.arrayBuffer()], { type: 'application/pdf' }), PDF_FILE_NAME)
      setDownload({ status: 'done' })
    } catch (error) {
      setDownload({ status: 'error', message: error instanceof Error ? error.message : String(error) })
    }
  }

  const busy = download.status === 'busy'

  return (
    <div className="shell">
      <header className="topbar">
        <div className="brand">
          <span className="brand-mark">AI</span>
          <span>
            AI Engineering <strong>Foundry</strong>
          </span>
        </div>
        <nav className="menu" aria-label="Main">
          <button
            type="button"
            className={view === 'preview' ? 'menu-item active' : 'menu-item'}
            aria-current={view === 'preview' ? 'page' : undefined}
            onClick={() => setView('preview')}
          >
            Preview policy
          </button>
          <button
            type="button"
            className={view === 'download' ? 'menu-item active' : 'menu-item'}
            aria-current={view === 'download' ? 'page' : undefined}
            onClick={downloadPdf}
            disabled={busy}
          >
            {busy ? 'Generating PDF…' : 'Download PDF'}
          </button>
        </nav>
      </header>

      <main className="content">
        {view === 'preview' ? (
          preview.status === 'ready' ? (
            <iframe className="preview" srcDoc={preview.html} title="AI Foundry Responsible AI Policy" />
          ) : (
            <section className="panel">
              {preview.status === 'loading' ? (
                <p className="status">Loading policy…</p>
              ) : (
                <>
                  <h1>Preview unavailable</h1>
                  <p className="status error">
                    Could not load {POLICY_HTML_URL}: {preview.message}. Make sure the latest htmltopdf-api service is
                    running on port 5291 (restart it after code changes).
                  </p>
                </>
              )}
            </section>
          )
        ) : (
          <section className="panel">
            <h1>Download policy PDF</h1>
            <p className="muted">
              AIF-POL-GOV-001 · Responsible AI Development and Use Policy · A4, 3 pages
            </p>
            {download.status === 'busy' && <p className="status">Rendering the PDF on the server, this takes a few seconds…</p>}
            {download.status === 'done' && <p className="status ok">Done. Check your downloads for {PDF_FILE_NAME}.</p>}
            {download.status === 'error' && (
              <p className="status error">
                Download failed: {download.message}. Is the htmltopdf-api service running on port 5291?
              </p>
            )}
            <div className="actions">
              <button type="button" className="primary" onClick={downloadPdf} disabled={busy}>
                {busy ? 'Generating…' : 'Download again'}
              </button>
              <button type="button" className="secondary" onClick={() => setView('preview')}>
                Back to preview
              </button>
            </div>
          </section>
        )}
      </main>
    </div>
  )
}

export default App
