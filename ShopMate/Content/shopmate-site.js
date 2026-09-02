(function (window, $) {
    'use strict';

    function migrateDataAttributes(root) {
        var scope = root || document;
        var mappings = [
            ['data-toggle', 'data-bs-toggle'],
            ['data-target', 'data-bs-target'],
            ['data-dismiss', 'data-bs-dismiss'],
            ['data-parent', 'data-bs-parent']
        ];

        mappings.forEach(function (pair) {
            var from = pair[0];
            var to = pair[1];
            scope.querySelectorAll('[' + from + ']').forEach(function (el) {
                if (!el.hasAttribute(to)) {
                    el.setAttribute(to, el.getAttribute(from));
                }
            });
        });
    }

    function initSidebar() {
        var sidebar = document.getElementById('appSidebar');
        var backdrop = document.getElementById('sidebarBackdrop');
        var toggle = document.getElementById('sidebarToggle');

        if (!sidebar || !toggle) {
            return;
        }

        function closeSidebar() {
            sidebar.classList.remove('is-open');
            if (backdrop) {
                backdrop.classList.remove('is-visible');
            }
        }

        function openSidebar() {
            sidebar.classList.add('is-open');
            if (backdrop) {
                backdrop.classList.add('is-visible');
            }
        }

        toggle.addEventListener('click', function () {
            if (sidebar.classList.contains('is-open')) {
                closeSidebar();
            } else {
                openSidebar();
            }
        });

        if (backdrop) {
            backdrop.addEventListener('click', closeSidebar);
        }
    }

    function initTreeview() {
        document.querySelectorAll('.sidebar-menu .treeview > a').forEach(function (link) {
            link.addEventListener('click', function (e) {
                if (link.getAttribute('href') === '#') {
                    e.preventDefault();
                }

                var parent = link.parentElement;
                var isOpen = parent.classList.contains('menu-open');

                document.querySelectorAll('.sidebar-menu .treeview.menu-open').forEach(function (item) {
                    if (item !== parent) {
                        item.classList.remove('menu-open');
                    }
                });

                if (isOpen) {
                    parent.classList.remove('menu-open');
                } else {
                    parent.classList.add('menu-open');
                }
            });
        });

        document.querySelectorAll('.sidebar-menu .treeview.active').forEach(function (item) {
            item.classList.add('menu-open');
        });
    }

    function initBoxWidgets() {
        document.querySelectorAll('[data-widget="collapse"]').forEach(function (btn) {
            btn.addEventListener('click', function (e) {
                e.preventDefault();
                var box = btn.closest('.box');
                if (!box) {
                    return;
                }
                var body = box.querySelector('.box-body');
                if (body) {
                    body.style.display = body.style.display === 'none' ? '' : 'none';
                }
            });
        });

        document.querySelectorAll('[data-widget="remove"]').forEach(function (btn) {
            btn.addEventListener('click', function (e) {
                e.preventDefault();
                var box = btn.closest('.box');
                if (box) {
                    box.remove();
                }
            });
        });
    }

    function patchJqueryBootstrap() {
        if (!$ || !window.bootstrap) {
            return;
        }

        var modalMethods = {
            show: function (inst) { inst.show(); },
            hide: function (inst) { inst.hide(); },
            toggle: function (inst) { inst.toggle(); },
            dispose: function (inst) { inst.dispose(); }
        };

        $.fn.modal = function (action) {
            return this.each(function () {
                var instance = bootstrap.Modal.getOrCreateInstance(this);
                if (action && modalMethods[action]) {
                    modalMethods[action](instance);
                }
            });
        };

        $.fn.dropdown = function (action) {
            return this.each(function () {
                var instance = bootstrap.Dropdown.getOrCreateInstance(this);
                if (action === 'toggle') {
                    instance.toggle();
                }
            });
        };

        $.fn.collapse = function (action) {
            return this.each(function () {
                var instance = bootstrap.Collapse.getOrCreateInstance(this);
                if (action === 'show') {
                    instance.show();
                } else if (action === 'hide') {
                    instance.hide();
                } else if (action === 'toggle') {
                    instance.toggle();
                }
            });
        };

        $.fn.tab = function (action) {
            return this.each(function () {
                var instance = bootstrap.Tab.getOrCreateInstance(this);
                if (action === 'show') {
                    instance.show();
                }
            });
        };
    }

    document.addEventListener('DOMContentLoaded', function () {
        migrateDataAttributes();
        initSidebar();
        initTreeview();
        initBoxWidgets();
        patchJqueryBootstrap();
    });

    if ($) {
        $(document).ready(function () {
            migrateDataAttributes(document);
            patchJqueryBootstrap();
        });
    }

    window.ShopMateReports = {
        initTable: function (selector, options) {
            if (!$ || !$.fn.DataTable) {
                return null;
            }

            var table = $(selector);
            if (!table.length || $.fn.DataTable.isDataTable(table)) {
                return table.length ? table.DataTable() : null;
            }

            var settings = $.extend(true, {
                dom: 'Bfrtip',
                buttons: ['copy', 'csv', 'excel', 'pdf', 'print'],
                pageLength: 25,
                language: {
                    search: '',
                    searchPlaceholder: 'Search report...'
                }
            }, options || {});

            return table.DataTable(settings);
        },

        initPrint: function (buttonSelector, containerSelector) {
            var btn = document.querySelector(buttonSelector || '#btnPrint');
            var container = document.querySelector(containerSelector || '#dvContainer');

            if (!btn || !container) {
                return;
            }

            btn.addEventListener('click', function (e) {
                e.preventDefault();
                var printWindow = window.open('', '_blank', 'height=700,width=900');
                if (!printWindow) {
                    return;
                }

                printWindow.document.write('<html><head><title>Report</title>');
                printWindow.document.write('<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />');
                printWindow.document.write('<style>body{font-family:system-ui,sans-serif;padding:24px;} table{width:100%;border-collapse:collapse;} th,td{border:1px solid #ddd;padding:8px;text-align:left;} th{background:#f4f6fb;}</style>');
                printWindow.document.write('</head><body>');
                printWindow.document.write(container.innerHTML);
                printWindow.document.write('</body></html>');
                printWindow.document.close();
                printWindow.focus();
                printWindow.print();
            });
        },

        initDateFilters: function (config) {
            if (!$) {
                return;
            }

            var opts = config || {};
            if (opts.isPosttime === '1') {
                $('#stime').val('00:01');
                $('#etime').val('23:59');
            }

            if ($.fn.datetimepicker) {
                $('#stime, #etime').datetimepicker({
                    datepicker: false,
                    format: 'H:i',
                    step: 5
                });
            }

            if ($.fn.datepicker) {
                $('#txtCal, #txtCalTo').datepicker();
            }
        },

        autoInit: function () {
            if (!$ || !$('.report-page').length) {
                return;
            }

            this.initPrint('#btnPrint', '#dvContainer');

            if ($('#tbInvoiceItem').length) {
                this.initTable('#tbInvoiceItem');
            }

            var postTime = document.getElementById('report-is-posttime');
            this.initDateFilters({
                isPosttime: postTime ? postTime.value : ''
            });
        }
    };

    document.addEventListener('DOMContentLoaded', function () {
        if (window.ShopMateReports) {
            window.ShopMateReports.autoInit();
        }
    });

    if ($) {
        $(document).ready(function () {
            if (window.ShopMateReports) {
                window.ShopMateReports.autoInit();
            }
        });
    }
})(window, window.jQuery);
