import DS from 'ember-data';

export default DS.ModelFragment.extend({
  personaName: DS.attr('string'),
  profileUrl: DS.attr('string'),
  avatar: DS.attr('string'),
  avatarMedium: DS.attr('string'),
  avatarFull: DS.attr('string'),
  personaState: DS.attr('number'),
});