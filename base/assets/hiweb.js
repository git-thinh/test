const Hiweb = (function () {
    function HiwebObject(options) {
        options = options || {};


        this.name = "SingletonTester";
        this.pointX = options.pointX || 6;
        this.pointY = options.pointY || 10;


        // Singleton

        // Private methods and variables
        function privateMethod() {
            console.log("I am private");
        }

        var privateVariable = "Im also private";

        var privateRandomNumber = Math.random();

        return {

            // Public methods and variables
            publicMethod: function () {
                console.log("The public can see me!");
            },

            publicProperty: "I am also public",

            getRandomNumber: function () {
                return privateRandomNumber;
            }

        };
    }

    var instance;
    return {
        getInstance: function (options) {
            if (instance === undefined) {
                instance = new HiwebObject(options);
            }
            return instance;
        }
    };
})();


const hiw = mySingleton.getInstance();