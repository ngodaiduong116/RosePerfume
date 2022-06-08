$('.dialog-link-customer').click(function (e) {
    var a_href = $(this).attr('href');
    console.log(a_href);
    e.preventDefault();
    $.ajax({
        url: a_href,
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $('#content-wrapper-customer').prepend(data);
            $('#Modal-detail-customer').modal('show');
        }
    });
});