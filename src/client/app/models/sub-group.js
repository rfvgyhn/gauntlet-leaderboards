import DS from 'ember-data';

export default DS.Model.extend({
  name: DS.attr(),
  leaderboards: DS.hasMany("leaderboard", { async: true })
});
