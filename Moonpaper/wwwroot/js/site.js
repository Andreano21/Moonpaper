//Ajax подгрузка статей
$(function () {

    $('div#loading').hide();

    var page = 0;
    var _inCallback = false;

    function loadItems() {
        if (page > -1 && !_inCallback) {
            _inCallback = true;
            page++;
            $('div#loading').show();

            var curentUrl = window.location.pathname + window.location.search;

            //Проверка на наличие GET параметров в URL
            if (curentUrl != "/") {
                curentUrl += "&Page=" + page;
            }
            else {
                curentUrl = window.location.href + "Home/All?Page=" + page;
            }

            $.ajax({
                type: 'GET',
                url: curentUrl,
                success: function (data, textstatus) {
                    if (data != '') {
                        $("#scrolList").append(data);
                    }
                    else {
                        page = -1;
                    }

                    _inCallback = false;

                    $("div#loading").hide();
                }
            });
        }
    }

    // обработка события скроллинга
    $(window).scroll(function () {
        if ($(window).scrollTop() == $(document).height() - $(window).height()) {

            loadItems();
        }
    });
})