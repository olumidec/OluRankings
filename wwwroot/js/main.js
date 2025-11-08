// =============================
//  OluRankings – micro-interactions
// =============================
(() => {
  const header = document.querySelector('.header');
  const onScroll = () => {
    if (!header) return;
    header.classList.toggle('scrolled', window.scrollY > 8);
  };
  onScroll();
  window.addEventListener('scroll', onScroll, { passive: true });

  // Mobile nav (optional—only if you have a toggle)
  const toggle = document.querySelector('[data-nav-toggle]');
  const mobile = document.querySelector('[data-nav-mobile]');
  if (toggle && mobile) {
    toggle.addEventListener('click', () => {
      mobile.toggleAttribute('data-open');
    });
  }

  // Subtle parallax on hero
  const hero = document.querySelector('.hero');
  if (hero) {
    window.addEventListener('scroll', () => {
      const y = Math.min(1, window.scrollY / 600);
      hero.style.backgroundPosition = `0 ${y * 30}px, 0 0, 0 0`;
    }, { passive: true });
  }

  // Show/hide password on auth forms
  document.querySelectorAll('[data-toggle-password]').forEach(btn=>{
    btn.addEventListener('click', ()=>{
      const input = document.querySelector(btn.dataset.togglePassword);
      if (input) {
        input.type = input.type === 'password' ? 'text' : 'password';
        btn.setAttribute('aria-pressed', input.type !== 'password');
      }
    });
  });
})();
