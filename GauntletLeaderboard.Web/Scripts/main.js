(function ($) {
    $(function () {
        $(".container[role=main] .nav a").each(function () {
            var $this = $(this);
            $this.removeClass("active");
            var path = location.href.substr(0, this.href.length).toLowerCase();
            var match = this.href.toLowerCase() == path;

            if (match)
                $this.add($this.parent("li")).addClass("active");
        });

        $("#search").on("input", function (e) {
            var $this = $(this);
            var q = $this.val();
            var $link = $this.next().find("a");

            $link.attr("href", $link.data("path") + "/" + q);
        }).on("keypress", function (e) {
            var $this = $(this);
            var pressedEnter = (e.keyCode || e.which) == 13;

            if (pressedEnter)
                $this.next().find("a")[0].click();
        });
    });
})(jQuery);