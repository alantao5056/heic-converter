# GEO Content Site: heicbatchconverter.alantao.com

Astro static site promoting HEIC Batch Converter (part of the GEO plan in
`docs/promo/PLAN.md`, Component 2). Landing page + guides, structured data
(SoftwareApplication / Article / FAQPage / HowTo), `llms.txt`, sitemap.

## Develop

```powershell
npm install
npm run dev       # http://localhost:4321
npm run build     # outputs static site to dist/
npm run preview   # serve dist/ locally
```

## Content

- Landing page: `src/pages/index.astro`
- Articles: Markdown in `src/content/articles/`; frontmatter schema in
  `src/content.config.ts` (title, description, pubDate, `tldr`, `faq`,
  optional `howto`). Adding a `.md` file automatically adds the page,
  the Guides card, and sitemap entry. Also add it to `public/llms.txt`
  and the Footer's Guides list.
- Shared constants (Store URL, support email): `src/site.ts`
- Design tokens: `src/styles/global.css`; palette follows
  `docs/ui-design/README.md` and `docs/posters/poster_1.png`

## Deploy

Static files only. DNS: `heicbatchconverter` A record → server IP.

- Caddy: merge `deploy/Caddyfile` into the server config (auto-HTTPS)
- Nginx: use `deploy/nginx.conf` + certbot

Auto-deploy (default): pushing to `main` with changes under `website/**` builds and
rsyncs `dist/` to the server via `.github/workflows/deploy-website.yml`. Requires these
repository secrets: `SSH_HOST`, `SSH_USER`, `SSH_PRIVATE_KEY`, `SSH_KNOWN_HOSTS`
(and optional `SSH_PORT`, `DEPLOY_PATH`). Runs can also be triggered manually from the
Actions tab.

Manual fallback: `DEPLOY_HOST=user@server ./scripts/deploy.sh` (rsyncs `dist/`).

## IndexNow

`public/2b071bd0fa8b02f44310a9076ed10dd1.txt` is the IndexNow key file (key = filename).
After deploying new/changed pages, push them to Bing:

```
POST https://api.indexnow.org/indexnow
{ "host": "heicbatchconverter.alantao.com",
  "key": "2b071bd0fa8b02f44310a9076ed10dd1",
  "urlList": ["https://heicbatchconverter.alantao.com/<changed-page>/"] }
```
