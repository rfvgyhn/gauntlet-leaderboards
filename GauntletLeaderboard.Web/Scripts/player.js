(function ($) {
    $(function () {
        $("table").DataTable({
            paging: false,
            info: false,
            search: { smart: false },
            order: [
                [1, "asc"],
                [2, "asc"],
                [3, "asc"],
            ],
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