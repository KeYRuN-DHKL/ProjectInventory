// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    // Initialize DataTables
    if ($.fn.DataTable) {
        $('.table').DataTable({
            "pageLength": 25,
            "order": [[0, "asc"]],
            "language": {
                "search": "Search:",
                "lengthMenu": "Show _MENU_ entries per page",
                "info": "Showing _START_ to _END_ of _TOTAL_ entries",
                "infoEmpty": "Showing 0 to 0 of 0 entries",
                "infoFiltered": "(filtered from _MAX_ total entries)",
                "paginate": {
                    "first": "First",
                    "last": "Last",
                    "next": "Next",
                    "previous": "Previous"
                }
            },
            "responsive": true
        });
    }

    // Auto-hide alerts after 5 seconds
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Confirm delete actions
    $('form[asp-action="Delete"]').on('submit', function (e) {
        if (!confirm('Are you sure you want to delete this item? This action cannot be undone.')) {
            e.preventDefault();
        }
    });

    // Confirm status toggle actions
    $('form[asp-action="ToggleStatus"]').on('submit', function (e) {
        if (!confirm('Are you sure you want to change the status of this item?')) {
            e.preventDefault();
        }
    });

    // Product code validation
    $('#Code').on('blur', function () {
        var code = $(this).val();
        var id = $('#Id').val();
        if (code) {
            $.post('/Products/CheckProductCode', { code: code, id: id })
                .done(function (data) {
                    if (!data.exists) {
                        $('#Code').addClass('is-invalid');
                        if (!$('#code-error').length) {
                            $('#Code').after('<div class="invalid-feedback" id="code-error">Product code must be unique.</div>');
                        }
                    } else {
                        $('#Code').removeClass('is-invalid');
                        $('#code-error').remove();
                    }
                });
        }
    });

    // Unit name validation
    $('#Name').on('blur', function () {
        var name = $(this).val();
        var id = $('#Id').val();
        if (name) {
            $.post('/Units/CheckUnitName', { name: name, id: id })
                .done(function (data) {
                    if (!data.exists) {
                        $('#Name').addClass('is-invalid');
                        if (!$('#name-error').length) {
                            $('#Name').after('<div class="invalid-feedback" id="name-error">Unit name must be unique.</div>');
                        }
                    } else {
                        $('#Name').removeClass('is-invalid');
                        $('#name-error').remove();
                    }
                });
        }
    });

    // Vendor PAN validation
    $('#PAN').on('blur', function () {
        var pan = $(this).val();
        var id = $('#Id').val();
        if (pan) {
            $.post('/Vendors/CheckPAN', { pan: pan, id: id })
                .done(function (data) {
                    if (!data.exists) {
                        $('#PAN').addClass('is-invalid');
                        if (!$('#pan-error').length) {
                            $('#PAN').after('<div class="invalid-feedback" id="pan-error">PAN number must be unique.</div>');
                        }
                    } else {
                        $('#PAN').removeClass('is-invalid');
                        $('#pan-error').remove();
                    }
                });
        }
    });

    // Invoice number validation
    $('#InvoiceNumber').on('blur', functi
