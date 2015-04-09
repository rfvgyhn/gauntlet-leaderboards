import DS from 'ember-data';

export default DS.Model.extend({
  group: DS.attr(),
  subGroup: DS.attr(),
  name: DS.attr(),
  special: DS.attr(),
  entries: DS.hasMany("entry", { async: true })
});
