using System.Text;

namespace HtmlToPdfApi;

/// <summary>
/// Builds the AIF-POL-GOV-001 policy document in code with a <see cref="StringBuilder"/>.
/// The output is the same markup as AI-Foundry-Policy-AIF-POL-GOV-001.html; the parts every
/// page repeats (letterhead, footer) are written once and appended per page.
/// </summary>
public static class PolicyHtmlBuilder
{
    private const int PageCount = 3;
    private const string StandardFooterNote = "Internal · Controlled Document";

    public static string Build(string logoDataUri)
    {
        var sb = new StringBuilder(capacity: 160_000);

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset=\"utf-8\">");
        sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
        sb.AppendLine("<title>AI Foundry Responsible AI Policy</title>");
        sb.AppendLine("<meta name=\"description\" content=\"AI Foundry Responsible AI Development and Use Policy, reference AIF-POL-GOV-001.\">");
        sb.AppendLine("<style>");
        sb.AppendLine(Styles);
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<main class=\"document\">");
        sb.AppendLine();

        AppendPage(sb, 1, logoDataUri, Page1Body, StandardFooterNote);
        AppendPage(sb, 2, logoDataUri, Page2Body, StandardFooterNote);
        AppendPage(sb, 3, logoDataUri, Page3Body, StandardFooterNote + " · Printed copies are uncontrolled");

        sb.AppendLine("</main>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        return sb.ToString();
    }

    private static void AppendPage(StringBuilder sb, int number, string logoDataUri, string body, string footerNote)
    {
        sb.AppendLine($"<!-- ======================= PAGE {number} ======================= -->");
        sb.AppendLine($"<article class=\"page\" aria-label=\"Page {number}\">");
        AppendLetterhead(sb, logoDataUri);
        sb.AppendLine();
        sb.AppendLine("  <div class=\"page-body\">");
        sb.AppendLine(body);
        sb.AppendLine("  </div>");
        sb.AppendLine();
        AppendFooter(sb, number, footerNote);
        sb.AppendLine("</article>");
        sb.AppendLine();
    }

    private static void AppendLetterhead(StringBuilder sb, string logoDataUri)
    {
        sb.AppendLine("  <header class=\"letterhead\">");
        sb.AppendLine("    <div class=\"brand\">");
        sb.AppendLine($"      <div class=\"logo-tile\"><img src=\"{logoDataUri}\" alt=\"AI Engineering Foundry logo\"></div>");
        sb.AppendLine("      <div>");
        sb.AppendLine("        <div class=\"brand-name\">AI Engineering <span>Foundry</span></div>");
        sb.AppendLine("        <div class=\"brand-sub\">Build. Engineer. Ship AI.</div>");
        sb.AppendLine("      </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("    <div class=\"lh-meta\">");
        sb.AppendLine("      <strong>Corporate Policy</strong>");
        sb.AppendLine("      Responsible AI Development &amp; Use<br>");
        sb.AppendLine("      <span class=\"ref\">AIF-POL-GOV-001 · v1.0 · aiengfoundry.com</span>");
        sb.AppendLine("    </div>");
        sb.AppendLine("  </header>");
    }

    private static void AppendFooter(StringBuilder sb, int number, string note)
    {
        sb.AppendLine("  <footer class=\"page-footer\">");
        sb.AppendLine("    <span><span class=\"ref\">AIF-POL-GOV-001</span> · Version 1.0</span>");
        sb.AppendLine($"    <span class=\"mid\">{note}</span>");
        sb.AppendLine($"    <span class=\"pg\">Page {number} of {PageCount}</span>");
        sb.AppendLine("  </footer>");
    }

    // Layout: fixed A4 sheets rendered with Blink under @media print (see Program.cs converter options).
    private const string Styles = """
        /* Layout: fixed A4 sheets, each carrying its own letterhead and footer; single paper look in both host themes by design */
        :root {
          --ink: #1b2a33;          /* primary text, headings */
          --ink-soft: #4a5a63;     /* secondary text */
          --accent: #0a55c4;       /* brand blue: rules, numbering, links */
          --accent-tint: #edf3fb;  /* table stripes, metadata ground */
          --navy: #0b1626;         /* letterhead band, table heads (site background) */
          --navy-2: #12243a;
          --cyan: #38d9f2;         /* brand cyan, used only on the dark band */
          --line: #d4dce6;         /* hairlines */
          --paper: #ffffff;
          --desk: #e3e8ef;         /* screen ground behind sheets */
          --warn: #8a5a12;  --warn-bg: #fbf5ea;  --warn-line: #d8b46e;
          --crit: #8c2f2b;  --crit-bg: #fbefee;  --crit-line: #d39591;
          --f-display: "Iowan Old Style", "Palatino Linotype", Palatino, "Book Antiqua", Georgia, serif;
          --f-body: "Segoe UI", "Helvetica Neue", Helvetica, Arial, system-ui, sans-serif;
          --f-brand: "Montserrat", "Segoe UI", "Helvetica Neue", Arial, sans-serif;
          --f-mono: "SFMono-Regular", Consolas, "Liberation Mono", Menlo, monospace;
          color-scheme: light;
        }
        * { box-sizing: border-box; }
        body {
          margin: 0; background: var(--desk); color: var(--ink);
          font-family: var(--f-body); font-size: 8.7pt; line-height: 1.45;
          padding-inline: 16px; padding-block: 28px;
          -webkit-print-color-adjust: exact; print-color-adjust: exact;
        }
        .document { display: grid; gap: 28px; justify-items: center; }

        /* ---------- Sheet ---------- */
        .page {
          width: 100%; max-width: 210mm; min-height: 297mm;
          background: var(--paper);
          box-shadow: 0 1px 2px rgba(27,42,51,.08), 0 8px 28px rgba(27,42,51,.10);
          padding: 11mm 15mm 9mm;
          display: flex; flex-direction: column;
        }
        .page-body { flex: 1; min-width: 0; padding-block: 5mm 3mm; }

        /* ---------- Letterhead: dark band matching aiengfoundry.com ---------- */
        .letterhead {
          display: flex; align-items: center; justify-content: space-between; gap: 16px; flex-wrap: wrap;
          margin: -11mm -15mm 0; padding: 6mm 15mm 5.6mm; position: relative; color: #fff;
          background:
            url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 320 120'%3E%3Cg stroke='%2338d9f2' stroke-opacity='.28' stroke-width='.6' fill='none'%3E%3Cpath d='M20 95L70 60L120 82L165 30L215 55L262 18L310 40'/%3E%3Cpath d='M70 60L95 12L165 30M120 82L178 100L215 55L250 92L310 70M262 18L310 40L310 70'/%3E%3Cpath d='M150 120V104H200L214 90H320M180 0V12H236L250 26H320' stroke-opacity='.22'/%3E%3C/g%3E%3Cg fill='%2338d9f2'%3E%3Ccircle cx='70' cy='60' r='1.8' fill-opacity='.6'/%3E%3Ccircle cx='165' cy='30' r='2.2' fill-opacity='.7'/%3E%3Ccircle cx='215' cy='55' r='1.6' fill-opacity='.55'/%3E%3Ccircle cx='262' cy='18' r='2' fill-opacity='.65'/%3E%3Ccircle cx='120' cy='82' r='1.4' fill-opacity='.45'/%3E%3Ccircle cx='310' cy='40' r='1.8' fill-opacity='.6'/%3E%3Ccircle cx='95' cy='12' r='1.2' fill-opacity='.4'/%3E%3Ccircle cx='250' cy='92' r='1.4' fill-opacity='.45'/%3E%3C/g%3E%3C/svg%3E") right -6mm center / auto 140% no-repeat,
            radial-gradient(120% 160% at 100% 0%, rgba(56,217,242,.16), transparent 55%),
            linear-gradient(100deg, var(--navy) 0%, var(--navy) 45%, var(--navy-2) 100%);
        }
        .letterhead::after { content: ""; position: absolute; left: 0; right: 0; bottom: 0; height: 1.4px; background: linear-gradient(90deg, var(--cyan) 0%, var(--accent) 55%, transparent 100%); }
        .brand { display: flex; align-items: center; gap: 3.4mm; min-width: 0; }
        .logo-tile { width: 13mm; height: 13mm; flex: none; background: #fff; border-radius: 2.4mm; display: grid; place-items: center; box-shadow: 0 0 0 .6px rgba(56,217,242,.55), 0 0 10px rgba(56,217,242,.28); }
        .logo-tile img { width: 10.4mm; height: auto; display: block; }
        .brand-name { font-family: var(--f-brand); font-weight: 800; font-size: 12.6pt; line-height: 1.05; letter-spacing: .03em; text-transform: uppercase; color: #fff; }
        .brand-name span { color: var(--cyan); }
        .brand-sub { font-family: var(--f-brand); font-size: 6.4pt; font-weight: 600; letter-spacing: .22em; text-transform: uppercase; color: #a9bdcc; margin-top: 1.4mm; }
        .lh-meta { text-align: right; font-size: 7.2pt; color: #b7c7d4; line-height: 1.5; position: relative; }
        .lh-meta strong { display: block; color: #fff; font-family: var(--f-brand); font-size: 7.4pt; font-weight: 700; letter-spacing: .14em; text-transform: uppercase; }
        .lh-meta .ref { font-family: var(--f-mono); font-size: 7pt; color: var(--cyan); }

        /* ---------- Footer ---------- */
        .page-footer {
          display: grid; grid-template-columns: 1fr auto 1fr; align-items: end; gap: 12px;
          border-top: .75px solid var(--line); padding-top: 3mm; position: relative;
          font-size: 6.9pt; color: var(--ink-soft); letter-spacing: .02em;
        }
        .page-footer::before { content: ""; position: absolute; left: 0; top: -1px; width: 24mm; height: 1.4px; background: linear-gradient(90deg, var(--cyan), var(--accent)); }
        .page-footer .ref { font-family: var(--f-mono); color: var(--accent); }
        .page-footer .mid { text-align: center; text-transform: uppercase; letter-spacing: .12em; font-weight: 600; color: var(--ink); font-size: 6.5pt; }
        .page-footer .pg { text-align: right; font-variant-numeric: tabular-nums; }

        /* ---------- Title block ---------- */
        .doc-type { font-size: 7pt; letter-spacing: .2em; text-transform: uppercase; color: var(--accent); font-weight: 600; margin: 0 0 2mm; }
        h1 { font-family: var(--f-display); font-weight: 500; font-size: 19pt; line-height: 1.15; margin: 0 0 1.5mm; text-wrap: balance; color: var(--ink); }
        .lede { font-size: 8.9pt; color: var(--ink-soft); max-width: 80ch; margin: 0 0 3.5mm; }
        .meta {
          display: grid; grid-template-columns: repeat(3, minmax(0, 1fr));
          border: .75px solid var(--line); background: var(--accent-tint); margin: 0 0 4mm;
        }
        .meta div { padding: 1.6mm 3mm; border-right: .75px solid var(--line); border-bottom: .75px solid var(--line); min-width: 0; }
        .meta div:nth-child(3n) { border-right: 0; }
        .meta div:nth-last-child(-n+3) { border-bottom: 0; }
        .meta dt { font-size: 6.4pt; letter-spacing: .14em; text-transform: uppercase; color: var(--ink-soft); margin-bottom: .6mm; }
        .meta dd { margin: 0; font-weight: 600; font-size: 8.8pt; font-variant-numeric: tabular-nums; }
        .meta dd.mono { font-family: var(--f-mono); font-weight: 500; font-size: 8.4pt; color: var(--accent); }
        .status { display: inline-block; padding: 0 2mm; border: .75px solid var(--accent); color: var(--accent); font-size: 7.4pt; letter-spacing: .08em; text-transform: uppercase; }

        /* ---------- Headings ---------- */
        h2 {
          display: flex; align-items: baseline; gap: 3mm;
          font-family: var(--f-display); font-weight: 600; font-size: 11.6pt; color: var(--ink);
          margin: 3.8mm 0 1.8mm; padding-bottom: 1.1mm; border-bottom: .75px solid var(--line);
          break-after: avoid;
        }
        h2 .num { font-family: var(--f-body); font-size: 9pt; font-weight: 700; color: var(--accent); letter-spacing: .04em; min-width: 6mm; }
        .page-body > h2:first-child { margin-top: 0; }
        h3 { font-size: 8.3pt; font-weight: 700; color: var(--ink); margin: 2.2mm 0 .8mm; break-after: avoid; }
        h3 .num { color: var(--accent); margin-right: 2mm; font-variant-numeric: tabular-nums; }
        p { margin: 0 0 1.4mm; }
        .ph { font-family: var(--f-mono); font-size: .9em; color: var(--accent); background: var(--accent-tint); padding: 0 1mm; white-space: nowrap; }

        /* ---------- Two-column groups ---------- */
        .cols { display: grid; grid-template-columns: 1fr 1fr; gap: 0 7mm; }
        .cols > * { min-width: 0; }

        /* ---------- Lists ---------- */
        ul, ol { margin: 0 0 1.4mm; padding-left: 5mm; }
        li { margin-bottom: .4mm; }
        ul { list-style: none; }
        ul > li { position: relative; }
        ul > li::before { content: ""; position: absolute; left: -4mm; top: .62em; width: 1.6mm; height: 1.6mm; background: var(--accent); }
        ul ul > li::before { background: transparent; border: .75px solid var(--accent); }
        .principles { counter-reset: p; padding: 0; list-style: none; }
        .principles li { counter-increment: p; padding-left: 8mm; }
        .principles li::before { content: counter(p, upper-roman) "."; background: none; width: auto; height: auto; top: 0; left: 0; font-weight: 700; color: var(--accent); font-size: 8pt; }

        /* Legal-style numbered requirements: 4.1, 4.1.1 */
        .req { counter-reset: r; list-style: none; padding: 0; }
        .req > li { counter-increment: r; padding-left: 9mm; position: relative; margin-bottom: 1mm; }
        .req > li::before { content: "4." counter(r); position: absolute; left: 0; top: 0; font-weight: 700; color: var(--accent); font-variant-numeric: tabular-nums; }
        .req > li > ol { counter-reset: s; list-style: none; padding: 0; margin-top: .8mm; }
        .req > li > ol > li { counter-increment: s; padding-left: 9mm; position: relative; }
        .req > li > ol > li::before { content: "4." counter(r) "." counter(s); position: absolute; left: 0; color: var(--ink-soft); font-size: 8pt; font-variant-numeric: tabular-nums; }
        .req > li > ol > li > ul { margin-top: .6mm; }
        .req strong { color: var(--ink); }

        .steps { counter-reset: st; list-style: none; padding: 0; }
        .steps > li { counter-increment: st; padding-left: 8mm; position: relative; margin-bottom: .8mm; }
        .steps > li::before {
          content: counter(st); position: absolute; left: 0; top: .05em; width: 4.6mm; height: 4.6mm;
          border: .75px solid var(--accent); color: var(--accent); font-size: 7pt; font-weight: 700;
          display: grid; place-items: center; line-height: 1;
        }

        /* ---------- Callouts ---------- */
        .callout { border: .75px solid var(--line); border-left: 2.5px solid var(--accent); background: var(--accent-tint); padding: 1.6mm 3mm; margin: 1.6mm 0 2mm; break-inside: avoid; }
        .callout .label { display: block; font-size: 6.6pt; letter-spacing: .16em; text-transform: uppercase; font-weight: 700; color: var(--accent); margin-bottom: .6mm; }
        .callout p:last-child { margin-bottom: 0; }
        .callout.warn { background: var(--warn-bg); border-color: var(--warn-line); border-left-color: var(--warn); }
        .callout.warn .label { color: var(--warn); }
        .callout.crit { background: var(--crit-bg); border-color: var(--crit-line); border-left-color: var(--crit); }
        .callout.crit .label { color: var(--crit); }

        /* ---------- Tables ---------- */
        .table-wrap { overflow-x: auto; margin: 1.2mm 0 2mm; }
        table { width: 100%; border-collapse: collapse; font-size: 7.6pt; line-height: 1.35; break-inside: auto; }
        thead { display: table-header-group; }
        tr { break-inside: avoid; }
        th {
          background: var(--navy); color: #fff; text-align: left; font-weight: 600;
          font-size: 6.9pt; letter-spacing: .1em; text-transform: uppercase; padding: 1.4mm 2.2mm; vertical-align: bottom;
        }
        td { padding: 1.1mm 2.2mm; border-bottom: .75px solid var(--line); vertical-align: top; }
        tbody tr:nth-child(even) td { background: var(--accent-tint); }
        td:first-child { font-weight: 600; }
        td.num, th.num { font-variant-numeric: tabular-nums; white-space: nowrap; }
        .raci { font-family: var(--f-mono); font-size: 7.2pt; font-weight: 700; color: var(--accent); white-space: nowrap; }
        .caption { font-size: 6.8pt; color: var(--ink-soft); margin: -1mm 0 2mm; }
        .tier { display: inline-block; font-size: 6.8pt; font-weight: 700; letter-spacing: .06em; text-transform: uppercase; padding: 0 1.4mm; border: .75px solid currentColor; white-space: nowrap; }
        .tier.t1 { color: var(--accent); } .tier.t2 { color: var(--warn); } .tier.t3 { color: var(--crit); }

        /* ---------- Sign-off ---------- */
        .signoff { display: grid; grid-template-columns: repeat(3, minmax(0, 1fr)); gap: 6mm; margin-top: 3mm; }
        .sig { border-top: .75px solid var(--ink); padding-top: 1.4mm; font-size: 7.4pt; color: var(--ink-soft); min-width: 0; }
        .sig b { display: block; color: var(--ink); font-size: 8pt; }
        .sig .line { height: 7mm; }

        /* ---------- Screen: narrow ---------- */
        @media screen and (max-width: 640px) {
          body { padding-block: 16px; }
          .page { min-height: 0; padding: 18px 16px 14px; }
          .cols, .signoff { grid-template-columns: 1fr; }
          .meta { grid-template-columns: repeat(2, minmax(0, 1fr)); }
          .meta div { border-right: .75px solid var(--line) !important; border-bottom: .75px solid var(--line) !important; }
          .page-footer { grid-template-columns: 1fr; text-align: left; }
          .page-footer .mid, .page-footer .pg { text-align: left; }
          .lh-meta { text-align: left; }
          .letterhead { margin: -18px -16px 0; padding: 16px; background-size: auto 100%, auto, auto; }
          h1 { font-size: 19pt; }
        }

        /* ---------- Print ---------- */
        @page { size: A4; margin: 0; }
        @media print {
          body { background: #fff; padding: 0; }
          .document { display: block; }
          .page {
            width: 210mm; max-width: none; height: 297mm; min-height: 0; overflow: hidden;
            box-shadow: none; margin: 0; break-after: page; page-break-after: always;
          }
          .page:last-child { break-after: auto; page-break-after: auto; }
          .table-wrap { overflow: visible; }
        }
        """;

    private const string Page1Body = """
            <p class="doc-type">Policy Document · Group-Wide</p>
            <h1>Responsible AI Development and Use Policy</h1>
            <p class="lede">This policy sets the rules AI Engineering Foundry (“AI Foundry”) follows when it designs, builds, buys, deploys and operates artificial intelligence systems, so that every system is lawful, safe, explainable and accountable throughout its lifecycle.</p>

            <dl class="meta">
              <div><dt>Document Title</dt><dd>Responsible AI Development and Use Policy</dd></div>
              <div><dt>Reference Number</dt><dd class="mono">AIF-POL-GOV-001</dd></div>
              <div><dt>Version</dt><dd>1.0</dd></div>
              <div><dt>Effective Date</dt><dd>01 November 2026</dd></div>
              <div><dt>Next Review Date</dt><dd>01 November 2027</dd></div>
              <div><dt>Document Status</dt><dd><span class="status">Approved</span></dd></div>
            </dl>

            <h2><span class="num">1</span>Policy Overview</h2>
            <h3><span class="num">1.1</span>Purpose</h3>
            <p>AI Foundry builds and operates AI systems that influence decisions about customers, employees and partners. This policy defines the minimum controls those systems must meet, assigns clear ownership for each control, and gives the Board assurance that AI-related risk is identified, measured and managed in line with <span class="ph">[Applicable Regulation / Framework]</span> and the organisation's risk appetite.</p>

            <h3><span class="num">1.2</span>Scope</h3>
            <div class="cols">
              <div>
                <p>This policy applies to:</p>
                <ul>
                  <li>All employees, contractors, interns and secondees of <span class="ph">[Organisation Legal Name]</span> and its subsidiaries.</li>
                  <li>All AI and machine-learning systems built in-house, procured from vendors, or accessed as a service, including generative AI and large language models.</li>
                </ul>
              </div>
              <div>
                <ul>
                  <li>Every lifecycle stage: ideation, data sourcing, training, validation, deployment, monitoring and retirement.</li>
                  <li>Personal and business use of public AI tools when handling AI Foundry information.</li>
                </ul>
                <p><em>Out of scope:</em> conventional rules-based automation with no learned component, unless it feeds an in-scope system.</p>
              </div>
            </div>

            <h3><span class="num">1.3</span>Objectives</h3>
            <ul>
              <li>Maintain a complete, current inventory of AI systems with an accountable owner for each.</li>
              <li>Classify every system by risk tier and apply controls proportionate to that tier.</li>
              <li>Prevent unlawful bias, privacy breaches and unsafe outputs before they reach production.</li>
              <li>Give management and regulators evidence that controls operate effectively.</li>
            </ul>

            <h2><span class="num">2</span>Policy Principles</h2>
            <h3><span class="num">2.1</span>Key Principles</h3>
            <ol class="principles">
              <li><strong>Accountability.</strong> A named human owner is answerable for each AI system and its outcomes.</li>
              <li><strong>Fairness.</strong> Systems are tested for disparate impact on protected groups before release and at regular intervals.</li>
              <li><strong>Transparency.</strong> People are told when they interact with AI and can obtain a meaningful explanation of material decisions.</li>
              <li><strong>Privacy and security by design.</strong> Data is minimised, protected and used only for its approved purpose.</li>
              <li><strong>Human oversight.</strong> High-impact decisions retain an effective human review and override.</li>
              <li><strong>Reliability.</strong> Performance, robustness and drift are measured against documented thresholds.</li>
            </ol>

            <div class="cols">
              <div>
                <h3><span class="num">2.2</span>Responsibilities</h3>
                <p>Every person in scope must use AI only for approved purposes, follow the requirements in Section 4, and report suspected incidents or policy breaches within one business day. Detailed accountabilities by role are set out in Section 3.</p>
              </div>
              <div>
                <h3><span class="num">2.3</span>Governance Requirements</h3>
                <ul>
                  <li>The AI Governance Committee (AIGC) oversees this policy and meets monthly.</li>
                  <li>All Tier 2 and Tier 3 systems require AIGC approval before deployment.</li>
                  <li>Material policy changes require Executive Committee approval.</li>
                </ul>
              </div>
            </div>
        """;

    private const string Page2Body = """
            <h2><span class="num">3</span>Roles and Responsibilities</h2>
            <div class="table-wrap">
              <table>
                <thead>
                  <tr><th style="width:22%">Role / Department</th><th style="width:20%">Owner</th><th>Accountability Areas</th><th style="width:9%">RACI</th></tr>
                </thead>
                <tbody>
                  <tr><td>Board of Directors</td><td>Board Risk Committee</td><td>Sets AI risk appetite; receives quarterly assurance reporting.</td><td class="raci">A</td></tr>
                  <tr><td>AI Governance Committee</td><td>Chief AI Officer (Chair)</td><td>Approves Tier 2–3 systems and exceptions; owns this policy; tracks remediation.</td><td class="raci">A / R</td></tr>
                  <tr><td>AI &amp; Data Science</td><td>Head of AI Engineering</td><td>Builds models to standard; documents model cards, data lineage and test evidence.</td><td class="raci">R</td></tr>
                  <tr><td>Business Units</td><td>System Owner (per system)</td><td>Defines intended use; owns outcomes, user communication and human oversight.</td><td class="raci">A</td></tr>
                  <tr><td>Information Security</td><td>Chief Information Security Officer</td><td>Threat modelling, access control, adversarial testing, incident response.</td><td class="raci">R / C</td></tr>
                  <tr><td>Data Protection</td><td>Data Protection Officer</td><td>Privacy impact assessments; lawful basis; data-subject rights.</td><td class="raci">C</td></tr>
                  <tr><td>Legal &amp; Compliance</td><td>General Counsel</td><td>Regulatory interpretation; contract clauses for AI vendors.</td><td class="raci">C</td></tr>
                  <tr><td>Procurement</td><td>Head of Procurement</td><td>Vendor due diligence; ensures third-party AI enters the inventory.</td><td class="raci">R</td></tr>
                  <tr><td>Internal Audit</td><td>Chief Audit Executive</td><td>Independent assurance over policy design and operating effectiveness.</td><td class="raci">I</td></tr>
                </tbody>
              </table>
            </div>
            <p class="caption">RACI: R = Responsible · A = Accountable · C = Consulted · I = Informed</p>

            <h2><span class="num">4</span>Policy Requirements</h2>
            <ol class="req">
              <li><strong>Inventory and classification.</strong> Register each AI system in the AI Register before development begins.
                <ol>
                  <li>Assign a risk tier using the AI Risk Classification Standard:
                    <ul>
                      <li><span class="tier t1">Tier 1</span> Low impact: internal productivity, no individual decisions.</li>
                      <li><span class="tier t2">Tier 2</span> Moderate: customer-facing or influences operational decisions.</li>
                      <li><span class="tier t3">Tier 3</span> High: affects rights, finances, employment or safety of individuals.</li>
                    </ul>
                  </li>
                  <li>Reassess the tier whenever the purpose, data or user population changes.</li>
                </ol>
              </li>
              <li><strong>Data management.</strong> Use only data with a documented lawful basis and approved purpose.
                <ol>
                  <li>Record data sources, lineage and retention period in the model documentation.</li>
                  <li>Apply anonymisation or pseudonymisation wherever identification is not required.</li>
                </ol>
              </li>
              <li><strong>Development and validation.</strong> Test every model against documented acceptance criteria.
                <ol>
                  <li>Tier 2–3 systems require bias, robustness and explainability testing, with results signed off by an independent validator.</li>
                  <li>Generative AI systems require red-team testing for harmful, inaccurate or leaked output.</li>
                </ol>
              </li>
            </ol>

            <div class="callout crit">
              <span class="label">Mandatory · Zero Tolerance</span>
              <p>Confidential, client or personal data must never be entered into public or unapproved AI tools. Use only services listed on the Approved AI Tools Register <span class="ph">[Intranet Link]</span>. Breaches are handled under the Disciplinary Policy.</p>
            </div>

            <ol class="req" start="4" style="counter-reset: r 3;">
              <li><strong>Deployment and oversight.</strong> No system goes live without the approvals for its tier.
                <ol>
                  <li>Tier 3 decisions must allow a trained human to review, override or reverse the outcome.</li>
                  <li>Users must be told clearly when content or a decision is produced by AI.</li>
                </ol>
              </li>
              <li><strong>Monitoring and retirement.</strong> Monitor performance, drift and complaints against thresholds, and decommission systems through a controlled process that preserves records.</li>
            </ol>

            <div class="callout warn">
              <span class="label">Important</span>
              <p>Report any AI incident, including harmful output, data leakage or unexplained performance degradation, to <span class="ph">[ai-incidents@organisation]</span> within <strong>24 hours</strong> of discovery.</p>
            </div>
        """;

    private const string Page3Body = """
            <h2><span class="num">5</span>Compliance and Governance</h2>
            <p>Compliance with this policy is mandatory. The Governance, Risk &amp; Compliance Office monitors adherence through the activities below and reports results to the AI Governance Committee.</p>
            <div class="table-wrap">
              <table>
                <thead>
                  <tr><th style="width:24%">Activity</th><th>Description</th><th style="width:17%">Frequency</th><th style="width:20%">Reported To</th></tr>
                </thead>
                <tbody>
                  <tr><td>Register attestation</td><td>System Owners confirm inventory entries and risk tiers are accurate.</td><td class="num">Quarterly</td><td>AIGC</td></tr>
                  <tr><td>Performance monitoring</td><td>Automated tracking of accuracy, drift and fairness metrics against thresholds.</td><td class="num">Continuous</td><td>System Owner</td></tr>
                  <tr><td>Compliance review</td><td>Sample-based testing of documentation, approvals and controls.</td><td class="num">Semi-annual</td><td>AIGC, Risk Committee</td></tr>
                  <tr><td>Management reporting</td><td>KPIs, incidents, open exceptions and remediation status.</td><td class="num">Quarterly</td><td>Executive Committee, Board</td></tr>
                  <tr><td>Independent audit</td><td>Assessment of policy design and operating effectiveness.</td><td class="num">Annual</td><td>Audit Committee</td></tr>
                  <tr><td>Policy review</td><td>Full review for regulatory, technology and business change.</td><td class="num">Annual</td><td>Executive Committee</td></tr>
                </tbody>
              </table>
            </div>
            <p><strong>Non-compliance.</strong> Breaches are logged, root-caused and remediated within agreed timelines. Repeated or deliberate breaches may lead to suspension of the AI system and disciplinary action.</p>

            <h2><span class="num">6</span>Exceptions and Escalation</h2>
            <div class="cols">
              <div>
                <ol class="steps">
                  <li><strong>Request.</strong> The System Owner submits an Exception Request Form stating the requirement, justification, risk and compensating controls.</li>
                  <li><strong>Review.</strong> GRC and the relevant specialists (Security, DPO, Legal) assess residual risk within 10 business days.</li>
                  <li><strong>Approve.</strong> The approver is set by residual risk, as shown opposite.</li>
                  <li><strong>Document.</strong> Approved exceptions are recorded in the Exceptions Register with an expiry date of no more than 12 months.</li>
                </ol>
              </div>
              <div>
                <div class="table-wrap" style="margin-top:0">
                  <table>
                    <thead><tr><th>Residual Risk</th><th>Approver</th></tr></thead>
                    <tbody>
                      <tr><td>Low</td><td>Head of GRC</td></tr>
                      <tr><td>Medium</td><td>AI Governance Committee</td></tr>
                      <tr><td>High</td><td>Executive Committee</td></tr>
                    </tbody>
                  </table>
                </div>
                <div class="callout">
                  <span class="label">Escalation</span>
                  <p>Disputed decisions escalate to the Chief AI Officer, then to the Executive Committee. Exceptions cannot be granted for legal or regulatory obligations.</p>
                </div>
              </div>
            </div>

            <h2><span class="num">7</span>Document Control</h2>
            <div class="table-wrap">
              <table>
                <thead>
                  <tr><th class="num">Version</th><th class="num">Date</th><th>Author / Owner</th><th>Reviewer</th><th>Approver</th><th>Change Description</th></tr>
                </thead>
                <tbody>
                  <tr><td class="num">0.1</td><td class="num">04 Aug 2026</td><td>GRC Policy Lead</td><td>Head of AI Engineering</td><td>—</td><td>Initial draft for consultation.</td></tr>
                  <tr><td class="num">0.9</td><td class="num">18 Sep 2026</td><td>GRC Policy Lead</td><td>DPO; CISO; Legal</td><td>—</td><td>Stakeholder comments incorporated; tiering added.</td></tr>
                  <tr><td class="num">1.0</td><td class="num">15 Oct 2026</td><td>Chief AI Officer</td><td>AI Governance Committee</td><td>Executive Committee</td><td>First approved release.</td></tr>
                </tbody>
              </table>
            </div>

            <div class="signoff">
              <div class="sig"><div class="line"></div><b>Policy Owner</b><span class="ph">[Name]</span>, Chief AI Officer</div>
              <div class="sig"><div class="line"></div><b>Reviewed By</b><span class="ph">[Name]</span>, Head of GRC</div>
              <div class="sig"><div class="line"></div><b>Approved By</b><span class="ph">[Name]</span>, Chief Executive Officer</div>
            </div>
        """;
}
