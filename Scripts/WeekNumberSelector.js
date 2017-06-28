$(document).ready(function () {

    $("#yearSelect").change(function () {
        var selectedYear = $("#yearSelect option:selected").val();
        var selectedWeek = $("#selectedWeekHolder").val();
        console.log(selectedWeek);
        var firstDay = new Date(selectedYear, 0, 1);
        var weekList = [];
        var firstFullWeekMonday;

        var currentDate = new Date();
        var week1 = new Date(currentDate.getFullYear(), 0, 4);

        var currentWeek = 1 + Math.round(((currentDate.getTime() - week1.getTime()) / 86400000 - 3 + (week1.getDay() + 6) % 7) / 7);


        switch (firstDay.getDay()) {
            case 0:
                //Sunday,
                weekList.push(getWeekListText(0, firstDay, 0));
                firstFullWeekMonday = firstDay;
                firstFullWeekMonday.setDate(firstFullWeekMonday.getDate() + 1);
                break;
            case 1:
                //Monday        
                weekList.push("N/A");
                firstFullWeekMonday = firstDay;

                break;
            case 2:
                weekList.push(getWeekListText(0, firstDay, 5));
                firstFullWeekMonday = firstDay;
                firstFullWeekMonday.setDate(firstFullWeekMonday.getDate() + 6);
                break;
            case 3:
                weekList.push(getWeekListText(0, firstDay, 4));
                firstFullWeekMonday = firstDay;
                firstFullWeekMonday.setDate(firstFullWeekMonday.getDate() + 5);
                break;
            case 4:
                weekList.push(getWeekListText(0, firstDay, 3));
                firstFullWeekMonday = firstDay;
                firstFullWeekMonday.setDate(firstFullWeekMonday.getDate() + 4);
                break;
            case 5:
                weekList.push(getWeekListText(0, firstDay, 2));
                firstFullWeekMonday = firstDay;
                firstFullWeekMonday.setDate(firstFullWeekMonday.getDate() + 3);
                break;
            case 6: //Saturday
                weekList.push(getWeekListText(0, firstDay, 1));
                firstFullWeekMonday = firstDay;
                firstFullWeekMonday.setDate(firstFullWeekMonday.getDate() + 2);
                break;
            default:
                break;
        }

        //Now loop to create listings for the weeks of the year
        for (j = 1; j < 52; j++) {
            weekList.push(getWeekListText(j, firstFullWeekMonday, 6));
            firstFullWeekMonday.setDate(firstFullWeekMonday.getDate() + 7);
        }

        lastDayOfYear = new Date(selectedYear, 11, 31);

        //Determine which day of the week the last day is
        //Create the last week listing with just the remaining days in the year.
        //firstFullWeekMonday is now the monday of the last week.
        if (lastDayOfYear.getDate() - firstFullWeekMonday.getDate() > 6) {
            //There is a week 53 to deal with.
            //Do week 52:
            weekList.push(getWeekListText(52, firstFullWeekMonday, 6));
            firstFullWeekMonday.setDate(firstFullWeekMonday.getDate() + 7);
            if (lastDayOfYear.getDay() == 0) {
                //Year ends on Sunday, so the last week is a full week.
                weekList.push(getWeekListText(53, firstFullWeekMonday, 6));
            }
            else {
                weekList.push(getWeekListText(53, firstFullWeekMonday, lastDayOfYear.getDay() - 1));
            }
        } else {
            if (lastDayOfYear.getDay() == 0) {
                //Year ends on Sunday, so the last week is a full week.
                weekList.push(getWeekListText(52, firstFullWeekMonday, 6));
            }
            else {
                weekList.push(getWeekListText(52, firstFullWeekMonday, lastDayOfYear.getDay() - 1));
            }
        }

        //Load the week selector dropdown
        $("#weekSelect").empty().append("");
        for (t = 0; t < weekList.length; t++) {

            if (selectedWeek == t) {
                $('#weekSelect').append($('<option>', {
                    value: t,
                    text: weekList[t],
                    selected: true
                }));
            } else {
                $('#weekSelect').append($('<option>', {
                    value: t,
                    text: weekList[t]
                }));
            }

        }

    }).change();

    $("#weekSelect").change(function () {
        $(this).closest('form').trigger('submit');
    });


    function getWeekListText(weekNumber, firstDay, weekLength) {
        var text = ""
        text += weekNumber + ": ";
        text += formatDate(firstDay);
        text += " to ";
        var weekEndDay = new Date(firstDay.getFullYear(), firstDay.getMonth(), firstDay.getDate());
        weekEndDay.setDate(weekEndDay.getDate() + weekLength);
        text += formatDate(weekEndDay);
        return text;
    }

    function formatDate(date) {
        var monthNames = [
            "January", "February", "March",
            "April", "May", "June", "July",
            "August", "September", "October",
            "November", "December"
        ];

        var dayName = ["Sunday", "Monday", "Tuesday", "Wednesday",
            "Thursday", "Friday", "Saturday"];
        var day = date.getDate();
        var monthIndex = date.getMonth();
        var year = date.getFullYear();
        var dayOfWeek = date.getDay();
        return dayName[dayOfWeek] + ', ' + day + ' ' + monthNames[monthIndex] + ' ' + year;
    }
});