<template>
  <div class="card-columns">
    <Light v-for="light in lights.filter(x => x.roomId == roomId || !roomId)" :key="light.deviceId" :device="light" />
    <Outlet v-for="outlet in outlets.filter(x => x.roomId == roomId || !roomId)" :key="outlet.deviceId" :device="outlet" />
    <Temperature v-for="temperature in temperatures.filter(x => x.roomId == roomId || !roomId)" :key="temperature.deviceId" :device="temperature" />
  </div>
</template>

<script>
import Light from '../../components/dashboard/Light.vue'
import Outlet from '../../components/dashboard/Outlet.vue'
import Temperature from '../../components/dashboard/Temperature.vue'
import { mapState } from 'vuex'
export default {
  props: ['roomId'],
  components: { Light, Outlet, Temperature },
  computed: mapState({
    lights: state => state.devices.all.filter(x => x.type == "light"),
    outlets: state => state.devices.all.filter(x => x.type == "outlet"),
    temperatures: state => state.devices.all.filter(x => x.type == "temperature")
  }),
  created () {
    this.$store.dispatch('getDevices')
  }
}
</script>

<style scoped>
.card-columns {
  column-count: 2;
}
@media (min-width: 768px) {
  .card-columns {
    column-count: 3;
  }
}
@media (min-width: 992px) {
  .card-columns {
    column-count: 4;
  }
}
@media (min-width: 1200px) {
  .card-columns {
    column-count: 5;
  }
}
</style>