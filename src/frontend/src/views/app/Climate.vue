<template>
  <Sensor v-for="sensor in devices.filter(x => x.type == 'temperature')" :key="sensor.deviceId" :device="sensor" />
  <Thermostat v-for="thermostat in devices.filter(x => x.type == 'fancoil' || x.type == 'trv')" :key="thermostat.deviceId" :device="thermostat" />
</template>
  
<script>
import Sensor from '../../components/app/Sensor.vue'
import Thermostat from '../../components/app/Thermostat.vue'
import { mapState } from 'vuex'
export default {
  components: { Sensor, Thermostat },
  computed: {
    ...mapState({
      allDevices: state => state.devices.all
    }),
    devices() {
      var roomId = this.$route.params.id
      return this.allDevices.filter(device => device.roomId == roomId)
        .filter(device => device.type == 'fancoil' || device.type == 'trv' || device.type == 'temperature')
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