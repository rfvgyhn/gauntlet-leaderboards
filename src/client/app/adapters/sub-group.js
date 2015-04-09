import ApplicationAdapter from './application';

export default ApplicationAdapter.extend({
    buildURL: function(type, id, snapshot) {
        if (id)
            id = id.replace(/\-/, " ");

        return this._super.apply(this, [type, id, snapshot]);
    }
});
