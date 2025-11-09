// Sticky header shadow
(function () {
  const header = document.querySelector('.site-header');
  if (!header) return;
  const drop = () => header.style.boxShadow = window.scrollY > 8 ? '0 8px 24px rgba(0,0,0,.35)' : 'none';
  drop(); window.addEventListener('scroll', drop);
})();

// Light play kick for muted hero video (mobile)
(function () {
  const v = document.querySelector('.hero video');
  if (!v) return;
  v.muted = true; v.playsInline = true;
  const playSafe = () => v.play().catch(()=>{ /* ignore autoplay blocks */ });
  document.addEventListener('visibilitychange', playSafe, { once:true });
  window.addEventListener('load', playSafe, { once:true });
})();
