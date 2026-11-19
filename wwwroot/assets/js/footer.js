function renderFooter() {
    const footerHTML = `
        <div class="f-top">
            <div class="f-left">
                <a href="index.html" class="f-brand">
                    <div class="logo">
                        <img src="assets/image/icon/logo/logo-dark.svg" alt="Home of Memories">
                    </div>
                    <h2>Home of Memories</h2>
                </a>
                <div class="f-nav">
                    <a class="a-anim" href="catalog.html" data-i18n="nav.find">Найти воспоминание</a>
                    <a class="a-anim" href="add-item.html" data-i18n="nav.add">Добавить воспоминание</a>
                    <a class="a-anim" href="item.html" data-i18n="nav.random">Случайное воспоминание</a>
                    <a class="a-anim" href="about.html" data-i18n="nav.about">О нас</a>
                </div>
            </div>
            <div class="f-right">
                <div class="socials">
                    <a class="social" href="https://www.instagram.com/" target="_blank">
                        <img src="assets/image/icon/social/instagram.svg" alt="Instagram">
                    </a>
                    <a class="social" href="https://web.telegram.org/" target="_blank">
                        <img src="assets/image/icon/social/telegram.svg" alt="Telegram">
                    </a>
                </div>
                <span>homeofmemories@gmail.com</span>
            </div>
        </div>
        <div class="f-bottom">
            <span data-i18n="footer.main">© Дом воспоминаний 2026</span>
            <span>Материалы взяты с <a class="a-anim" href="https://unsplash.com/" target="_blank">unsplash.com</a>
                и <a class="a-anim" href="https://www.svgrepo.com/" target="_blank">svgrepo.com</a></span>
        </div>
    `;
    const footerElement = document.querySelector('footer');
    if (footerElement) {
        footerElement.innerHTML = footerHTML;
    }
}