import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // Forward /api/* to the htmltopdf-api ASP.NET Core service (see htmltopdf-api/Properties/launchSettings.json).
    proxy: {
      '/api': {
        target: process.env.HTMLTOPDF_API_URL ?? 'http://localhost:5291',
        rewrite: (path) => path.replace(/^\/api/, ''),
      },
    },
  },
})
