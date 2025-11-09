(() => {
  // ---- THEME (preserve existing setting)
  const root = document.documentElement;
  const toggle = document.getElementById('themeToggle');
  const saved = localStorage.getItem('theme');
  if (saved === 'light' || saved === 'dark') root.setAttribute('data-theme', saved);
  toggle && toggle.addEventListener('click', () => {
    const next = root.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
    root.setAttribute('data-theme', next);
    localStorage.setItem('theme', next);
  });

  // ---- MOBILE NAV (simple reveal)
  const ham = document.getElementById('hamburger');
  const nav = document.querySelector('.nav-links');
  ham && ham.addEventListener('click', () => {
    const shown = getComputedStyle(nav).display !== 'none';
    nav.style.display = shown ? 'none' : 'flex';
  });

  // ---- HIGHLIGHTS (keeps your JSON flow)
  const grid = document.getElementById('highlightsGrid');
  if (grid) {
    fetch('/data/highlights.json', { cache: 'no-cache' })
      .then(r => r.json())
      .then(items => {
        grid.innerHTML = items.map(renderHighlight).join('');
        wireHighlightClicks(items);
      })
      .catch(() => {
        grid.innerHTML = `<div class="muted">No highlights available.</div>`;
      });
  }

  function renderHighlight(h) {
    const thumb = (h.thumbnail || '').trim() || `https://img.youtube.com/vi/${extractYouTubeId(h.url)}/hqdefault.jpg`;
    return `
      <article class="h-card" data-id="${h.id}">
        <div class="h-thumb">
          <img alt="${escapeHtml(h.title)}" src="${thumb}">
          <button class="h-play" data-id="${h.id}">Play ▶</button>
        </div>
        <div class="h-meta">
          <div class="h-title">${escapeHtml(h.title)}</div>
          <div class="h-source">${escapeHtml(h.source || '')}</div>
        </div>
      </article>
    `;
  }

  function wireHighlightClicks(items) {
    const map = Object.fromEntries(items.map(i => [String(i.id), i]));
    const modal = document.getElementById('videoModal');
    const close = modal?.querySelector('.modal-close');
    const player = document.getElementById('modalPlayer');
    const title = document.getElementById('modalTitle');

    grid.addEventListener('click', (e) => {
      const btn = e.target.closest('.h-play');
      if (!btn) return;
      const id = String(btn.dataset.id);
      const item = map[id];
      if (!item) return;

      const yt = extractYouTubeId(item.url);
      const embed = yt ? `https://www.youtube.com/embed/${yt}?autoplay=1&rel=0` : item.url;
      player.innerHTML = `<iframe src="${embed}" allow="autoplay; encrypted-media; picture-in-picture" allowfullscreen></iframe>`;
      title.textContent = item.title || '';
      modal.showModal();
    });

    close?.addEventListener('click', () => {
      player.innerHTML = '';
      modal.close();
    });
    modal?.addEventListener('close', () => { player.innerHTML = ''; });
  }

  function extractYouTubeId(url=''){
    const m = url.match(/(?:youtube\.com\/watch\?v=|youtu\.be\/)([A-Za-z0-9_-]{6,})/);
    return m ? m[1] : '';
  }
  function escapeHtml(s=''){
    return s.replace(/[&<>"']/g, m => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[m]));
  }
})();
