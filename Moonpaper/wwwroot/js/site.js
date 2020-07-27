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
//В каком виде отображаются статьи на текущий момент
var viewStatus = "grid";


//<body onresize="pageSizeListener()">
function pageSizeListener() {
    if (viewStatus === "line") {
        //Определение выводимых в линию тегов исходя из размеров окна
        var w = window.innerWidth;
        console.log(w);
        var countTags = 0;
        if (w > 1000) {
            countTags = 4;
            TagsToLineBlock(countTags);
        }
        else if (w > 900) {
            countTags = 3;
            TagsToLineBlock(countTags);
        }
        else if (w > 800) {
            countTags = 2;
            TagsToLineBlock(countTags);
        }
        else if (w > 740) {
            countTags = 1;
            TagsToLineBlock(countTags);
        }
    }
}

//Устанавливает отображение статей в линию
function SetupToLine() {
    var elements = document.querySelectorAll(".block__portion_grid");
    elements.forEach(element => {
        element.className = "block__portion_line";
    });

    var elements = document.querySelectorAll(".block__item_grid");
    elements.forEach(element => {
        element.className = "block__item_line";
    });

    //Переключение активной кнопки в меню
    var b = document.querySelector(".menu__right").querySelector(".menu__button.menu__button_right.active");
    b.className = "menu__button menu__button_right";

    var c = document.querySelector(".menu__right").querySelector(".menu__button.menu__button_left");
    c.className = "menu__button menu__button_left active";

    //Переключение активной кнопки в боковом меню
    var d = document.querySelector(".side-menu").querySelector("#linkRadioToLine");
    d.className = "link__radio active";

    var e = document.querySelector(".side-menu").querySelector("#linkRadioToGrid");
    e.className = "link__radio";

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

    viewStatus = "line";
}

//Устанавливает отображение статей по сетке
function SetupToGrid() {
    var elements = document.querySelectorAll(".block__portion_line");
    elements.forEach(element => {
        element.className = "block__portion_grid";
    });

    var elements = document.querySelectorAll(".block__item_line");
    elements.forEach(element => {
        element.className = "block__item_grid";
    });

    //Переключение активной кнопки в меню
    var b = document.querySelector(".menu__right").querySelector(".menu__button_left.active");
    b.className = "menu__button menu__button_left";

    var c = document.querySelector(".menu__right").querySelector(".menu__button_right");
    c.className = "menu__button menu__button_right active";

    //Переключение активной кнопки в боковом меню
    var d = document.querySelector(".side-menu").querySelector("#linkRadioToLine");
    d.className = "link__radio";

    var e = document.querySelector(".side-menu").querySelector("#linkRadioToGrid");
    e.className = "link__radio active";

    TagsToDropBlock();

    viewStatus = "grid";
}

//Перебрасывает теги в линейный блок
function TagsToLineBlock(tagsCount) {
    var tagBlocks = document.querySelectorAll(".tags");

    tagBlocks.forEach(tagBlock => {
        var tags = tagBlock.querySelectorAll(".tag");
        var tags__Line = tagBlock.querySelector(".tags__Line");

        //Выводит в линейный блок  первые tagsCount тега, остальные в выпадающий блок
        if (tags.length > tagsCount) {
            for (var i = 0; i < tagsCount; i++) {
                tags__Line.appendChild(tags[i]);
            }
        }
        else {
            tags.forEach(tag => {

                tags__Line.appendChild(tag);
            });
        }

        //Скрывает блок tags__Drop если в нем нет тегов
        if (tags.length <= tagsCount) {
            var tags__Drop_element = tagBlock.querySelector("#displayable");
            tags__Drop_element.className = "tags__Drop_disable";
        }
        else {
            var tags__Drop_element = tagBlock.querySelector("#displayable");
            tags__Drop_element.className = "tags__Drop";
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