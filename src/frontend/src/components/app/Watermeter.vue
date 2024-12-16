<template>
  <div>
    <div class="card mb-3 p-3">
      <div class="card-body">
        <svg class="bi me-2" width="16" height="16">
          <use xlink:href="#droplet"/>
        </svg>
        <p class="name">
          {{ device.name }}
        </p>
        <p class="fw-bold fs-4 mb-1">
          {{ device.totalLiters }} l
        </p>
        <p class="fw-bold fs-4 mb-1">
          {{ device.litersPerMinute }} lpm
        </p>
        {{ getAgo(device.lastSeen) }}
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: ['device'],
  created () {
    var self = this;
    setInterval(function() {
      self.$forceUpdate()
    }, 1000);
  }
}
</script>

<style scoped>
@import '../../assets/card.css';
</style>