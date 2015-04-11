(function ($) {
    $(function () {
        $(".container[role=main] .nav a").each(function () {
            var $this = $(this);
            $this.removeClass("active");
            var path = location.href.substr(0, this.href.length).toLowerCase();
            var match = this.href.toLowerCase() == path;

            if (match)
                $this.add($this.parent("li")).addClass("active");
        })
    });
})(jQuery);