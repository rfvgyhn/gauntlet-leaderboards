(function ($) {
    $(function () {
        $("table").DataTable({
            paging: false,
            info: false,
            language: {
                search: "",
                searchPlaceholder: "Filter"
            },
            initComplete: function () {
                $(".dataTables_filter input").addClass("form-control input-sm");
            }
        });
    })
})(jQuery);