document.addEventListener("DOMContentLoaded", () => {
    const profileBtn = document.getElementById('profileBtn');

    if (!profileBtn) return;

    // модальное окно
    const modalHTML = `
        <div id="authModal" class="modal-overlay" style="display: none;">
            <div class="modal-content">
                <span class="close-btn" id="closeAuthBtn">&times;</span>
                <h2 class="modal-title" data-i18n="main">Дом воспоминаний</h2>

                <!-- ВХОД -->
                <div id="loginBlock">
                    <form id="loginForm" class="auth-form">
                        <input type="text" name="login" class="form-input" placeholder="Логин" required>
                        <input type="password" name="password" class="form-input" placeholder="Пароль" required>
                        <button type="submit" class="btn btn-dark" style="width: 100%; margin-top: 10px;">Войти</button>
                    </form>
                    <p class="switch-auth-text">
                        Ещё нет аккаунта? <a href="#" id="goToRegister">Зарегистрироваться</a>
                    </p>
                </div>

                <!-- РЕГИСТАЦИЯ -->
                <div id="registerBlock" style="display: none;">
                    <form id="registerForm" class="auth-form">
                        <input type="text" name="login" class="form-input" placeholder="Придумайте логин" required>
                        <input type="email" name="email" class="form-input" placeholder="Email" required>
                        <input type="password" name="password" class="form-input" placeholder="Пароль" required>
                        <button type="submit" class="btn btn-dark" style="width: 100%; margin-top: 10px;">Зарегистрироваться</button>
                    </form>
                    <p class="switch-auth-text">
                        Уже есть аккаунт? <a href="#" id="goToLogin">Войти</a>
                    </p>
                </div>
            </div>
        </div>
    `;
    document.body.insertAdjacentHTML('beforeend', modalHTML);

    const modal = document.getElementById('authModal');
    const closeBtn = document.getElementById('closeAuthBtn');
    const loginBlock = document.getElementById('loginBlock');
    const registerBlock = document.getElementById('registerBlock');
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');

    // переключение между входом и регистрацией
    document.getElementById('goToRegister').addEventListener('click', (e) => {
        e.preventDefault();
        loginBlock.style.display = 'none';
        registerBlock.style.display = 'block';
    });

    document.getElementById('goToLogin').addEventListener('click', (e) => {
        e.preventDefault();
        registerBlock.style.display = 'none';
        loginBlock.style.display = 'block';
    });

    // закрытие окна
    closeBtn.addEventListener('click', () => {
        modal.style.display = 'none';
    });
    window.addEventListener('click', (e) => {
        if (e.target === modal) {
            modal.style.display = 'none';
        }
    });

    // проверка авторизации
    profileBtn.addEventListener('click', async () => {
        try {
            const response = await fetch('/api/auth/me');
            if (response.ok) {
                window.location.href = 'my-home.html';
            } else {
                registerBlock.style.display = 'none';
                loginBlock.style.display = 'block';
                modal.style.display = 'flex';
            }
        } catch (e) {
            modal.style.display = 'flex';
        }
    });

    // отправка данных из формы вход
    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const data = Object.fromEntries(new FormData(loginForm).entries());

        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            window.location.href = 'my-home.html';
        } else {
            const error = await response.json();
            alert(error.error || 'Ошибка входа');
        }
    });

    // отправка данных из формы егистрация
    registerForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const data = Object.fromEntries(new FormData(registerForm).entries());

        const response = await fetch('/api/auth/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            alert('Регистрация успешна! Теперь войдите.');
            // автоматическое переключение на форму входа
            registerBlock.style.display = 'none';
            loginBlock.style.display = 'block';
            registerForm.reset();
        } else {
            const error = await response.json();
            alert(error.error || 'Ошибка регистрации');
        }
    });
});