import Ember from 'ember';

export default Ember.Route.extend({
    model: function(params) {
        return this.store.find("sub-group", params["sub-group_id"]).get("leaderboards");
    }
});
