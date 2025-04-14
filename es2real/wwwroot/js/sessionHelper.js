window.sessionHelper = {
    saveUserId: function (userId) {
        sessionStorage.setItem("userId", userId);
    },
    getUserId: function () {
        return sessionStorage.getItem("userId");
    },
    clearUserId: function () {
        sessionStorage.removeItem("userId");
    }
};
