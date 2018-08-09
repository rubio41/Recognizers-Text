# Microsoft.Recognizers.Text for JavaScript

## Getting Started

Recognizer's are organized into groups and designed to be used in C# and Node.js to help you build great applications!

To use the samples, install the `recognizers-text-suite` package.

## Installation from NPM

Install all the Recognizer's by launching the following command:

* Get **all** the Recognizers in a single module:
`npm install recognizers-text-suite`

Or, if you prefer to use a single type of recognizer:

* Get **only** the numbers Recognizer's features:
`npm install recognizers-text-number`

* Get **only** the numbers with units Recognizer's features:
`npm install recognizers-text-number-with-unit`

* Get **only** the date and time Recognizer's features:
`npm install recognizers-text-date-time`

## API Documentation

Once the proper package is installed, you'll need to reference the package:

````JavaScript
var Recognizers = require('recognizers-text-suite');
````

### Microsoft.Recognizers.Text.Number

* [NumberModel](https://github.com/Microsoft/Recognizers-Text/tree/master/JavaScript/packages/recognizers-number/src/number/numberRecognizer.ts)

    This recognizer will find any number from the input. E.g. "I have two apples" will return "2".

    `Recognizers.NumberRecognizer.instance.getNumberModel(Recognizers.Culture.English)`

* [OrdinalModel](https://github.com/Microsoft/Recognizers-Text/tree/master/JavaScript/packages/recognizers-number/src/number/numberRecognizer.ts)

    This recognizer will find any ordinal number. E.g. "eleventh" will return "11".

    `Recognizers.NumberRecognizer.instance.getOrdinalModel(Recognizers.Culture.English)`

* [PercentageModel](https://github.com/Microsoft/Recognizers-Text/tree/master/JavaScript/packages/recognizers-number/src/number/numberRecognizer.ts)

    This recognizer will find any number presented as percentage. E.g. "one hundred percents" will return "100%".

    `Recognizers.NumberRecognizer.instance.getPercentageModel(Recognizers.Culture.English)`

### Microsoft.Recognizers.Text.NumberWithUnit

* [AgeModel](https://github.com/Microsoft/Recognizers-Text/tree/master/JavaScript/packages/recognizers-number-with-unit/src/numberWithUnit/numberWithUnitRecognizer.ts)

    This recognizer will find any age number presented. E.g. "After ninety five years of age, perspectives change" will return "95 Year".

    `Recognizers.NumberWithUnitRecognizer.instance.getAgeModel(Recognizers.Culture.English)`

* [CurrencyModel](https://github.com/Microsoft/Recognizers-Text/tree/master/JavaScript/packages/recognizers-number-with-unit/src/numberWithUnit/numberWithUnitRecognizer.ts)

    This recognizer will find any currency presented. E.g. "Interest expense in the 1988 third quarter was $ 75.3 million" will return "75300000 Dollar".

    `Recognizers.NumberWithUnitRecognizer.instance.getCurrencyModel(Recognizers.Culture.English)`

* [DimensionModel](https://github.com/Microsoft/Recognizers-Text/tree/master/JavaScript/packages/recognizers-number-with-unit/src/numberWithUnit/numberWithUnitRecognizer.ts)

    This recognizer will find any dimension presented. E.g. "The six-mile trip to my airport hotel that had taken 20 minutes earlier in the day took more than three hours." will return "6 Mile".

    `Recognizers.NumberWithUnitRecognizer.instance.getDimensionModel(Recognizers.Culture.English)`

* [TemperatureModel](https://github.com/Microsoft/Recognizers-Text/tree/master/JavaScript/packages/recognizers-number-with-unit/src/numberWithUnit/numberWithUnitRecognizer.ts)

    This recognizer will find any temperature presented. E.g. "Set the temperature to 30 degrees celsius" will return "30 C".

    `Recognizers.NumberWithUnitRecognizer.instance.getTemperatureModel(Recognizers.Culture.English)`

### Microsoft.Recognizers.Text.DateTime

* [DateTimeModel](https://github.com/Microsoft/Recognizers-Text/tree/master/JavaScript/packages/recognizers-date-time/src/dateTime/dateTimeRecognizer.ts)

    This model will find any date, time, duration and date/time ranges, even if its write in coloquial language. E.g. "I'll go back 8pm today" will return "2017-10-04 20:00:00".

    `Recognizers.DateTimeRecognizer.instance.getDateTimeModel(Recognizers.Culture.English)`

## Samples

[Start using recognizers!](https://github.com/Microsoft/Recognizers-Text/tree/master/JavaScript/samples)