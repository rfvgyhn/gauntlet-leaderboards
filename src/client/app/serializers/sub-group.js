import DS from 'ember-data';

export default DS.RESTSerializer.extend({
    normalize: function(type, hash) {
        hash.id = hash.id.dasherize();

        return this._super.apply(this, arguments);
    },
    serialize: function() {
        var json = this._super.apply(this, arguments);
        json.id = json.id.replace(/\-/, "");

        return json;
    }
});
