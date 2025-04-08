window.confirmNavigation = {
    enable: function () {
        window.onbeforeunload = function () {
            return "編集中の内容が破棄されますが、よろしいですか？";
        };
    },
    disable: function () {
        window.onbeforeunload = null;
    }
};
