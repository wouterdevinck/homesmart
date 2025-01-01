export default {
  methods: {
    getUnitFormat: function(unit) {
      if(unit == '%') {
        return unit
      } else {
        return ' ' + unit
      }
    }
  }
}
