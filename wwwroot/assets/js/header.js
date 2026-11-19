function renderHeader() {
    const headerHTML = `
<div class="h-top">
    <a href="index.html" class="logo">
        <img src="assets/image/icon/logo/logo-light.svg" alt="Home of Memories">
    </a>
    <div class="search">
        <div class="search-icon">
            <img src="assets/image/icon/interact/search.svg" alt="Поиск">
        </div>
        <input type="text" placeholder="Поиск" />
    </div>
    <div class="header-actions">
        <button id="langToggle" class="lang-btn">EN</button>
        <button id="profileBtn" class="account"</button>
    </div>
</div>
<nav class="nav">
    <a class="a-anim" href="catalog.html" data-i18n="nav.find">Найти воспоминание</a>
    <a class="a-anim" href="add-item.html" data-i18n="nav.add">Добавить воспоминание</a>
    <a class="a-anim" href="item.html" data-i18n="nav.random">Случайное воспоминание</a>
    <a class="a-anim" href="about.html" data-i18n="nav.about">О нас</a>
</nav>
    `;
    const headerElement = document.querySelector('header');
    if (headerElement) {
        headerElement.innerHTML = headerHTML;
    }
}