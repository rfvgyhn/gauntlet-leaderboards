import Ember from 'ember';
import config from './config/environment';

var Router = Ember.Router.extend({
  location: config.locationType
});

export default Router.map(function() {
  this.route("index", { path: "/" }, function() {
      this.resource("sub-group", { path: "/:group_id" }, function() {
          this.resource("leaderboards", { path: "/:sub-group_id"}, function() {
            this.resource("leaderboard", { path: "/:leaderboard_id" });
          });    
      });
  });
});
