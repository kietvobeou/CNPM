// Tự động chạy carousel mỗi 5 giây
document.addEventListener('DOMContentLoaded', function () {
    var promotionCarousel = document.getElementById('promotionCarousel');
    if (promotionCarousel) {
        var carousel = new bootstrap.Carousel(promotionCarousel, {
            interval: 5000, // 5 giây
            ride: 'carousel',
            wrap: true
        });
    }

    // Thêm hiệu ứng click cho các promotion images
    var promotionLinks = document.querySelectorAll('.promotion-link');
    promotionLinks.forEach(function (link) {
        link.addEventListener('click', function (e) {
            // Có thể thêm hiệu ứng loading hoặc tracking ở đây
            console.log('Đang chuyển hướng đến chi tiết khuyến mãi');
        });
    });
});