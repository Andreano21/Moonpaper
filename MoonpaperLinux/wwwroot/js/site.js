//В каком виде отображаются статьи на текущий момент
var viewStatus = "line";

//Установка отображения статей в соответствии с найтройкой
$(document).ready(function () {

    //Загрузка из куки настроек вывода новостей
    var vs = GetTypeOfOutput("viewStatus");

    //проверка viewStatus в кукис
    if (vs !== "") {
        viewStatus = vs;
        SetupToLine();
    }

    if (viewStatus === "line") {
        SetupToLine();
    }
    else if (viewStatus === "grid") {
        SetupToGrid();
    }

})

function GetTypeOfOutput(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function SetTypeOfOutput(cArg) {
    var d = new Date();
    d.setTime(d.getTime() + (1000 * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = "viewStatus" + "=" + cArg + ";" + expires;
}

//Устанавливает отображение статей в линию
function SetupToLine() {
    var element = document.querySelector(".block_grid");

    if (element !== null) {
        element.className = "block_line";

        var elements = document.querySelectorAll(".block__item_grid");
        elements.forEach(element => {
            element.className = "block__item_line";
        });

        //Переключение активной кнопки в меню
        var b = document.querySelector(".menu__right").querySelector(".menu__button.menu__button_right.active");
        b.className = "menu__button menu__button_right";

        var c = document.querySelector(".menu__right").querySelector(".menu__button.menu__button_left");
        c.className = "menu__button menu__button_left active";

        //Определение выводимых в линию тегов исходя из размеров окна
        TagsToLineBlock();

        SetTypeOfOutput("line");
        viewStatus = "line";
    }

    //Переключение активной кнопки в мобильном меню
    var b = document.querySelector(".view-switch");
    b.setAttribute('onclick', 'SetupToGrid();');
    b.querySelector('img').setAttribute('src', '/img/icons/i_menu_grid.png');
}

//Устанавливает отображение подгруженных AJAX статей в линию
function SetupToLineItem() {

    var elements = document.querySelectorAll(".block__item_grid");
    elements.forEach(element => {
        element.className = "block__item_line";
    });

    //Определение выводимых в линию тегов исходя из размеров окна
    var w = window.innerWidth;
    var countTags = 0;
    if (w > 1000) {
        countTags = 4;
    }
    else if (w > 900) {
        countTags = 3;
    }
    else if (w > 800) {
        countTags = 2;
    }
    else if (w > 740) {
        countTags = 1;
        TagsToLineBlock(countTags);
    }

    TagsToLineBlock(countTags);

}

//Устанавливает отображение статей по сетке
function SetupToGrid() {
    var element = document.querySelector(".block_line");

    if (element !== null) {
        element.className = "block_grid";

        var elements = document.querySelectorAll(".block__item_line");
        elements.forEach(element => {
            element.className = "block__item_grid";
        });

        //Переключение активной кнопки в меню
        var b = document.querySelector(".menu__right").querySelector(".menu__button_left.active");
        b.className = "menu__button menu__button_left";

        var c = document.querySelector(".menu__right").querySelector(".menu__button_right");
        c.className = "menu__button menu__button_right active";

        TagsToDropBlock();

        SetTypeOfOutput("grid");
        viewStatus = "grid";
    }

    //Переключение активной кнопки в мобильном меню
    var b = document.querySelector(".view-switch");
    b.setAttribute('onclick', 'SetupToLine();');
    b.querySelector('img').setAttribute('src', '/img/icons/i_menu_lines.png');
}

//Перебрасывает теги для которых есть место в линейный блок, остальные в выпадающий
function TagsToLineBlock() {
    var articleWidth = document.querySelector('.block_line').clientWidth;
    var imgWidth = 260;
    var infoWidth;
    var availableWidth;

    var tagBlocks = document.querySelectorAll(".tags");

    tagBlocks.forEach(tagBlock => {
        var tags = tagBlock.querySelectorAll(".tag");
        var tags__Line = tagBlock.querySelector(".tags__Line");
        var tags__Drop_panel = tagBlock.querySelector(".tags__Drop_panel");


        //предварительный вывод в линейный блок
        tags.forEach(tag => {
            tags__Line.appendChild(tag);
        });
        var tags__Drop_element = tagBlock.querySelector("#displayable");
        tags__Drop_element.className = "tags__Drop_disable";

        infoWidth = tagBlock.parentElement.querySelector(".info").clientWidth + 15;

        availableWidth = articleWidth - imgWidth - infoWidth;
        var curentFreeSpace = 80;

        var tagInDropPanel = 0;
        tags.forEach(tag => {

            curentFreeSpace += tag.clientWidth + 10;

            if (curentFreeSpace > availableWidth) {
                tags__Drop_panel.appendChild(tag);
                tagInDropPanel++;
            }
        });

        //Скрывает блок tags__Drop если в нем нет тегов
        if (tagInDropPanel >= 1) {
            var tags__Drop_element = tagBlock.querySelector("#displayable");
            tags__Drop_element.className = "tags__Drop";
        }
        else {
            var tags__Drop_element = tagBlock.querySelector("#displayable");
            tags__Drop_element.className = "tags__Drop_disable";
        }
    });
}

//Перебрасывает теги в выпадающий блок
function TagsToDropBlock() {
    var tagBlocks = document.querySelectorAll(".tags");

    tagBlocks.forEach(tagBlock => {
        var tags = tagBlock.querySelectorAll(".tag");
        var tags__Drop_panel = tagBlock.querySelector(".tags__Drop_panel");

        tags.forEach(tag => {
            tags__Drop_panel.appendChild(tag);
        });

        //сброс отображения блока
        var tags__Drop_element = tagBlock.querySelector("#displayable");
        tags__Drop_element.className = "tags__Drop";
    });
}

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

                    if (viewStatus === "line") {
                        SetupToLineItem();
                    }
                }
            });
        }
    }

    //обработка события скроллинга
    $(window).scroll(function () {
        var position = $(document).height() - $(window).scrollTop() - $(window).height();

        //Размер документа может быть дробным числом, а размер окна только целым
        //Так же мобильный браузер может выдавать значение на 50-60 пикселей больше чем реальное
        //Исходя из этих условий задан следующий параметр
        if (position < 100) {
            loadItems();
        }
    });
})

//Установка кнопок лайков в новое состояние после нажатия
$(document).ready(function () {
    $(document).on('submit', '.info__stars-button', function () {

        //Изменение счетчика звезд на стороне клиента
        if ($(this).hasClass('active')) {
            var starsElement = $(this).parent().find('.info__stars');
            var stars = starsElement[0].innerText;
            var starsNum = Number(stars);

            if (starsNum !== NaN) {
                starsNum--;
                starsElement[0].innerText = starsNum;
            }
        }
        else {
            var starsElement = $(this).parent().find('.info__stars');
            var stars = starsElement[0].innerText;
            var starsNum = Number(stars);

            if (starsNum !== NaN) {
                starsNum++;
                starsElement[0].innerText = starsNum;
            }
        }

        //Изменение отображение статуса кнопки и post запроса
        if ($(this).hasClass('active')) {
            $(this).removeClass('active');
            $(this).attr('action', '/Home/StarUp');
        }
        else {
            $(this).addClass('active');
            $(this).attr('action', '/Home/StarDown');
        }

    });
});

//Установка кнопок подписок в новое состояние после нажатия
$(document).ready(function () {
    $(document).on('submit', '.tag__btn-on', function () {

        var $tagBlock = $(this).parent();

        //очистка статусов формы
        $tagBlock.find('.tag__btn-off').removeClass('active');
        $tagBlock.find('.tag__btn-on').removeClass('active');
        $tagBlock.find('.tag__name').removeClass('disactive');
        $tagBlock.find('.tag__name').removeClass('active');

        //установка новых статусов
        $(this).addClass('active');
        $tagBlock.find('.tag__name').addClass('active');

    });

    $(document).on('submit', '.tag__btn-off', function () {

        var $tagBlock = $(this).parent();

        //очистка статусов формы
        $tagBlock.find('.tag__btn-off').removeClass('active');
        $tagBlock.find('.tag__btn-on').removeClass('active');
        $tagBlock.find('.tag__name').removeClass('disactive');
        $tagBlock.find('.tag__name').removeClass('active');

        //установка новых статусов
        $(this).addClass('active');
        $tagBlock.find('.tag__name').addClass('disactive');

    });
});

//Кнопка подъема в начало страницы
var btn = $('#button_rise');

$(window).scroll(function () {
    if ($(window).scrollTop() > 300) {
        btn.addClass('show');
    } else {
        btn.removeClass('show');
    }
});

btn.on('click', function (e) {
    e.preventDefault();
    $('html, body').animate({ scrollTop: 0 }, '300');
});

