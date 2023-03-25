$(document).ready(function () {
    // Load countries data from JSON file
    $.getJSON('https://datahub.io/core/world-cities/r/world-cities.json', function (data) {
        // Remove duplicates and sort by country name
        var countries = [...new Set(data.map(city => city.country))].sort();

        // Populate countries select options
        var countrySelect = $('#country');
        countries.forEach(function (country) {
            countrySelect.append($('<option>', {
                value: country,
                text: country
            }));
        });

        // When a country is selected, populate cities select options
        countrySelect.on('change', function () {

            $('#CountrySpanId').text("");

            var country = $(this).val();
            var cities = data.filter(city => city.country === country).map(city => city.name).sort();

            var citySelect = $('#city');
            citySelect.empty();
            citySelect.append($('<option>', {
                value: '',
                text: 'Select a city'
            }));
            cities.forEach(function (city) {
                citySelect.append($('<option>', {
                    value: city,
                    text: city
                }));
            });

            citySelect.on('change', function () {
                $('#CitySpanId').text("");
            })
        });
    });

    $('#SubmitFormButtonId').click(() => {
        var country = $('#country').val();
        var city = $('#city').val();

        if (country && city) {

            $('#CountrySpanId').text("");
            $('#CitySpanId').text("");

            //Assign the selected values to the real input fields
            $('#RealCountryIF').val(country);
            $('#RealCityIF').val(city);


            $('#RealSubmitFormButtonId').click();
        } else {
            $('#CountrySpanId').text("Please select the country");
            $('#CitySpanId').text("Please select the city");
        }
    });
});