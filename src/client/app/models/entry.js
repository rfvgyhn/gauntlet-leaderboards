import DS from 'ember-data';

export default DS.Model.extend({
  player: DS.hasOneFragment("player"),
  rank: DS.attr(),
  score: DS.attr()
});
