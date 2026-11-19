document.addEventListener("DOMContentLoaded", () => {
    const langBtn = document.getElementById('langToggle');
    let currentLang = localStorage.getItem('siteLang') || 'ru';
    let dictionary = {};
    const translatedCache = new Map(); // локальный кеш в памяти вкладки

    if (langBtn && currentLang === 'en') {
        langBtn.textContent = 'RU';
    }

    // отслеживание появления новых элементов на странице
    const observer = new MutationObserver((mutations) => {
        if (currentLang !== 'en') return;
        for (const mutation of mutations) {
            for (const node of mutation.addedNodes) {
                if (node.nodeType === Node.ELEMENT_NODE) {
                    translateNodeTree(node, 'en');
                }
            }
        }
    });

    observer.observe(document.body, { childList: true, subtree: true });

    if (currentLang === 'en') {
        initTranslation('en');
    }

    if (langBtn) {
        langBtn.addEventListener('click', () => {
            if (currentLang === 'ru') {
                currentLang = 'en';
                langBtn.textContent = 'RU';
                localStorage.setItem('siteLang', 'en');
                initTranslation('en');
            } else {
                localStorage.setItem('siteLang', 'ru');
                window.location.reload();
            }
        });
    }

    async function initTranslation(lang) {
        try {
            const dictRes = await fetch(`/api/translate/dictionary?lang=${lang}`);
            if (dictRes.ok) {
                dictionary = await dictRes.json();
            }
        } catch (error) {
            console.error("Ошибка загрузки словаря:", error);
        }
        await translateNodeTree(document.body, lang);
    }

    async function translateNodeTree(rootElement, lang) {
        if (lang !== 'en') return;

        // перевод по словарю
        const i18nElements = rootElement.matches?.('[data-i18n]')
            ? [rootElement, ...rootElement.querySelectorAll('[data-i18n]')]
            : rootElement.querySelectorAll('[data-i18n]');

        i18nElements.forEach(el => {
            const key = el.getAttribute('data-i18n');
            if (dictionary[key]) {
                el.textContent = dictionary[key];
                el.dataset.translated = 'true';
            }
        });

        // плейсхолдеры
        const inputs = rootElement.querySelectorAll('input[placeholder], textarea[placeholder]');
        for (const input of inputs) {
            const ph = input.getAttribute('placeholder');
            if (ph && /[а-яА-ЯёЁ]/.test(ph)) {
                input.setAttribute('placeholder', await fetchTranslation(ph, lang));
            }
        }

        // обход всех текстовых узлов, не ломая верстку
        const walker = document.createTreeWalker(
            rootElement,
            NodeFilter.SHOW_TEXT,
            {
                acceptNode: (node) => {
                    const parent = node.parentElement;
                    if (!parent) return NodeFilter.FILTER_REJECT;
                    // пропуск скриптов, стилей и уже переведенных по словарю блоков
                    if (['SCRIPT', 'STYLE', 'NOSCRIPT', 'TEXTAREA'].includes(parent.tagName)) {
                        return NodeFilter.FILTER_REJECT;
                    }
                    if (parent.closest('[data-translated="true"]')) {
                        return NodeFilter.FILTER_REJECT;
                    }
                    const text = node.nodeValue.trim();
                    // перевод тех узлов, где есть кириллица
                    if (!text || !/[а-яА-ЯёЁ]/.test(text)) {
                        return NodeFilter.FILTER_REJECT;
                    }
                    return NodeFilter.FILTER_ACCEPT;
                }
            }
        );

        const textNodes = [];
        let currentNode;
        while ((currentNode = walker.nextNode())) {
            textNodes.push(currentNode);
        }

        for (const textNode of textNodes) {
            const originalText = textNode.nodeValue;
            const trimmedText = originalText.trim();
            const translated = await fetchTranslation(trimmedText, lang);

            if (translated && translated !== trimmedText) {
                // исходные пробелы по краям
                textNode.nodeValue = originalText.replace(trimmedText, translated);
            }
        }
    }

    async function fetchTranslation(text, lang) {
        if (translatedCache.has(text)) {
            return translatedCache.get(text);
        }

        const startTime = performance.now();
        try {
            const res = await fetch(`/api/translate?text=${encodeURIComponent(text)}&lang=${lang}`);
            if (res.ok) {
                const data = await res.json();
                translatedCache.set(text, data.translated);

                const duration = performance.now() - startTime;
                if (duration > 100) {
                    await new Promise(resolve => setTimeout(resolve, 150));
                }
                return data.translated;
            }
        } catch (err) {
            console.error("Ошибка перевода:", text);
        }
        return text;
    }
});
