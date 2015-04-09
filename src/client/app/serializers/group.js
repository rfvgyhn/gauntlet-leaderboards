import DS from 'ember-data';

export default DS.RESTSerializer.extend({
    normalize: function(type, hash) {
        hash.id = hash.id.dasherize();

        return this._super.apply(this, arguments);
    }
});
