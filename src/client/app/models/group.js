import DS from 'ember-data';

export default DS.Model.extend({
    name: DS.attr(),
    subGroups: DS.hasMany("sub-group", { async: true })
});
