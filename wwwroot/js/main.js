// wwwroot/js/main.js

document.addEventListener('DOMContentLoaded', () => {
  // Mobile nav toggle
  const btn = document.getElementById('navToggle');
  const menu = document.getElementById('navLinks');
  if (btn && menu) btn.addEventListener('click', () => menu.classList.toggle('open'));

  // Light/Dark theme toggle (html.light)
  const toggle = document.getElementById('themeToggle');
  const root = document.documentElement;
  const KEY = 'olurank-theme';

  const setTheme = (mode) => {
    if (mode === 'light') root.classList.add('light');
    else root.classList.remove('light');
    localStorage.setItem(KEY, mode);
  };

  // init from saved
  const saved = localStorage.getItem(KEY);
  if (saved) setTheme(saved);

  if (toggle) {
    toggle.addEventListener('click', () => {
      const isLight = root.classList.toggle('light');
      localStorage.setItem(KEY, isLight ? 'light' : 'dark');
    });
  }

  // Mark active nav link (simple contains check)
  const links = document.querySelectorAll('.nav-link[href]');
  const here = location.pathname.toLowerCase();
  links.forEach(a => {
    const href = a.getAttribute('href')?.toLowerCase() || '';
    if (href && href !== '/' && here.startsWith(href)) a.classList.add('active');
    if (href === '/' && here === '/') a.classList.add('active');
  });

  // Load highlights on landing (if present)
  loadHighlights();
});

// ---- Trending Highlights ----
async function loadHighlights() {
  const grid = document.getElementById('highlightsGrid');
  if (!grid) return;

  try {
    const res = await fetch('/data/highlights.json', { cache: 'no-cache' });
    const items = await res.json();

    grid.innerHTML = items.map(item => `
      <article class="h-card" data-type="${item.type}">
        <div class="h-thumb" role="img" aria-label="${item.title}"
             style="background-image:url('${item.thumb || ''}')"></div>
        <div class="h-body">
          <h3>${item.title}</h3>
          <p class="muted">${item.meta || ''}</p>
          <button class="btn btn-primary" data-open="${item.id}">Watch</button>
        </div>
      </article>
    `).join('');

    grid.addEventListener('click', (e) => {
      const btn = e.target.closest('button[data-open]');
      if (!btn) return;
      const id = btn.getAttribute('data-open');
      const item = items.find(x => x.id === id);
      openHighlight(item);
    });
  } catch (err) {
    grid.innerHTML = `<p class="muted">Unable to load highlights right now.</p>`;
    console.error(err);
  }
}

function openHighlight(item) {
  const modal = ensureModal();
  let embed = '';

  if (item.type === 'youtube') {
    const url = new URL(item.url);
    const v = url.searchParams.get('v') || item.url.split('/').pop();
    embed = `<iframe width="100%" height="420" src="https://www.youtube.com/embed/${v}" frameborder="0" allowfullscreen></iframe>`;
  } else if (item.type === 'tiktok') {
    embed = `
      <blockquote class="tiktok-embed" cite="${item.url}" style="max-width: 605px; min-width: 325px;">
        <a href="${item.url}"></a>
      </blockquote>
      <script async src="https://www.tiktok.com/embed.js"></script>`;
  }

  modal.querySelector('.m-content').innerHTML = `<h3 style="margin-top:0">${item.title}</h3>${embed}`;
  modal.classList.add('open');
}

function ensureModal() {
  let el = document.getElementById('modal');
  if (el) return el;
  el = document.createElement('div');
  el.id = 'modal';
  el.innerHTML = `
    <div class="m-backdrop"></div>
    <div class="m-card">
      <button class="m-close" aria-label="Close">✕</button>
      <div class="m-content"></div>
    </div>`;
  document.body.appendChild(el);

  el.querySelector('.m-backdrop').addEventListener('click', () => el.classList.remove('open'));
  el.querySelector('.m-close').addEventListener('click', () => el.classList.remove('open'));
  return el;
}
