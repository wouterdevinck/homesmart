<template>
  <Plug v-for="outlet in devices.filter(x => x.type == 'outlet')" :key="outlet.deviceId" :device="outlet" />
</template>
  
<script>
import Plug from '../../components/app/Plug.vue'
import { mapState } from 'vuex'
export default {
  components: { Plug },
  computed: {
    ...mapState({
      allDevices: state => state.devices.all
    }),
    devices() {
      var roomId = this.$route.params.id
      return this.allDevices.filter(device => device.roomId == roomId)
        .filter(device => (device.type == 'outlet' && device.energy) || device.type == 'solar')
    }
  },
  created () {
    this.$store.dispatch('getDevices')
  }
}
</script>
  
<style scoped>

</style>