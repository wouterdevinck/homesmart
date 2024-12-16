<template>
  <Plug v-for="outlet in devices.filter(x => x.type == 'outlet')" :key="outlet.deviceId" :device="outlet" />
  <Watermeter v-for="meter in devices.filter(x => x.type == 'watermeter')" :key="meter.deviceId" :device="meter" />
</template>
  
<script>
import Plug from '../../components/app/Plug.vue'
import Watermeter from '../../components/app/Watermeter.vue';
import { mapState } from 'vuex'
export default {
  components: { Plug, Watermeter },
  computed: {
    ...mapState({
      allDevices: state => state.devices.all
    }),
    devices() {
      var roomId = this.$route.params.id
      return this.allDevices.filter(device => device.roomId == roomId)
        .filter(device => (device.type == 'outlet' && device.energy) || device.type == 'solar' || device.type == 'watermeter')
    }
  },
  created () {
    this.$store.dispatch('getDevices')
  }
}
</script>
  
<style scoped>

</style>