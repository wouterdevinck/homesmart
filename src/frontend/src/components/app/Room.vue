<template>
  <div>
    <div class="card mb-3">
      <div class="card-body p-2"> 
        <div class="roomname"> 
          <svg class="icon">
            <use v-bind:xlink:href="'#' + getRoomIcon(room)"/>
          </svg>
          <span class="ms-2">{{ room.name }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: ['room']
}
</script>

<style scoped>
@import '../../assets/card.css';
.roomname {
  font-weight: bold;
}
.roomname > * {
  vertical-align: middle;
}
.icon {
  width: 50px;
  height: 50px;
}
@media (max-width: 400px) {
  .icon {
    width: 45px;
    height: 45px;
  }
}
</style>