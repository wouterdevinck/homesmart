<template>
  <Temperature v-for="temperature in devices.filter(x => x.type == 'temperature')" :key="temperature.deviceId" :device="temperature" />
</template>
  
<script>
import Temperature from '../../components/dashboard/Temperature.vue'
import { mapState } from 'vuex'
export default {
  components: { Temperature },
  computed: {
    ...mapState({
      allDevices: state => state.devices.all
    }),
    devices() {
      var roomId = this.$route.params.id
      return this.allDevices.filter(device => device.roomId == roomId)
        .filter(device => device.type == 'fancoil' || device.type == 'trv' || device.type == 'temperature')
    }
  },
  created () {
    this.$store.dispatch('getDevices')
  }
}
</script>
  
<style scoped>

</style>