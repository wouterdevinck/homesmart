<template>
  <Light v-for="light in devices.filter(x => x.type == 'light')" :key="light.deviceId" :device="light" />
</template>
  
<script>
import Light from '../../components/app/Light.vue'
import { mapState } from 'vuex'
export default {
  components: { Light },
  computed: {
    ...mapState({
      allDevices: state => state.devices.all
    }),
    devices() {
      var roomId = this.$route.params.id
      return this.allDevices.filter(device => device.roomId == roomId)
        .filter(device => device.type == 'light' || (device.type == 'outlet' && !device.energy))
    }
  },
  created () {
    this.$store.dispatch('getDevices')
  }
}
</script>
  
<style scoped>

</style>