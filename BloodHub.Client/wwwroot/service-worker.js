const CACHE_NAME = 'bloodhub-cache-v1';
const urlsToCache = [
    '/',
    '/index.html',
    //'/css/styles.css',
    //'/js/scripts.js',
    //'/images/logo.png'
];

// Cài đặt Service Worker và thêm tài nguyên vào cache
self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => {
                console.log('Opened cache');
                return cache.addAll(urlsToCache);
            })
    );
});

// Kích hoạt Service Worker và làm sạch cache cũ
self.addEventListener('activate', event => {
    const cacheWhitelist = [CACHE_NAME];
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.map(cacheName => {
                    if (!cacheWhitelist.includes(cacheName)) {
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
});

// Lắng nghe các yêu cầu fetch và trả về tài nguyên từ cache nếu có
self.addEventListener('fetch', event => {
    event.respondWith(
        caches.match(event.request)
            .then(response => {
                if (response) {
                    return response; // Trả về tài nguyên từ cache
                }
                return fetch(event.request); // Thực hiện yêu cầu mạng nếu tài nguyên không có trong cache
            })
            .catch(() => {
                if (event.request.mode === 'navigate') {
                    return caches.match('/index.html'); // Trả về trang index.html nếu yêu cầu thất bại
                }
            })
    );
});

// Xử lý thông báo đẩy (Push Notifications)
self.addEventListener('push', event => {
    const data = event.data.json();
    const options = {
        body: data.body,
    //    icon: 'images/icon.png',
    //    badge: 'images/badge.png'
    };
    event.waitUntil(
        self.registration.showNotification(data.title, options)
    );
});

// Xử lý sự kiện nhấp vào thông báo (Notification Click)
self.addEventListener('notificationclick', event => {
    event.notification.close();
    event.waitUntil(
        clients.openWindow(event.notification.data.url)
    );
});
