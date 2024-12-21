<template>
  <Light v-for="light in devices.filter(x => x.type == 'light')" :key="light.deviceId" :device="light" />
  <Plug v-for="outlet in devices.filter(x => x.type == 'outlet')" :key="outlet.deviceId" :device="outlet" />
</template>
  
<script>
import Light from '../../components/app/Light.vue'
import Plug from '../../components/app/Plug.vue'
import { mapState } from 'vuex'
export default {
  components: { Light, Plug },
  computed: {
    ...mapState({
      allDevices: state => state.devices.all
    }),
    devices() {
      var roomId = this.$route.params.id
      return this.allDevices.filter(device => device.roomId == roomId)
        .filter(device => device.type == 'light' || (device.type == 'outlet' && !device.energy))
        .filter(device => !(!device.reachable && device.hideWhenUnreachable))
    }
  },
  created () {
    this.$store.dispatch('getDevices')
  }
}
</script>
  
<style scoped>

</style>