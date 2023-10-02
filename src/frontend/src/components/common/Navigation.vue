<template>
  <nav class="d-flex flex-column flex-shrink-0 p-3 bg-light">
    <ul class="nav nav-pills flex-column mb-auto">
      <li>
        <router-link to="/" class="nav-link" active-class="active">
          <svg class="bi me-2" width="16" height="16"><use xlink:href="#phone"/></svg>
          App
        </router-link>
      </li>
      <li>
        <router-link to="/dashboard" class="nav-link" active-class="active">
          <svg class="bi me-2" width="16" height="16"><use xlink:href="#columns-gap"/></svg>
          Dashboard
        </router-link>
      </li>
      <li>
        <router-link to="/devices" href="#" class="nav-link" active-class="active">
          <svg class="bi me-2" width="16" height="16"><use xlink:href="#table"/></svg>
          Devices
        </router-link>
      </li>
      <li>
        <router-link to="/automations" href="#" class="nav-link" active-class="active">
          <svg class="bi me-2" width="16" height="16"><use xlink:href="#gear"/></svg>
          Automations
        </router-link>
      </li>
      <li>
        <router-link to="/battery" href="#" class="nav-link" active-class="active">
          <svg class="bi me-2" width="16" height="16"><use xlink:href="#battery-half"/></svg>
          Battery
        </router-link>
      </li>
      <li>
        <router-link to="/development" href="#" class="nav-link" active-class="active">
          <svg class="bi me-2" width="16" height="16"><use xlink:href="#code"/></svg>
          Development
        </router-link>
      </li>
    </ul>
  </nav>
</template>

<style scoped>
nav {
  width: 200px;
}
.nav-link {
  color: black;
}
@media (max-width: 576px) {
  nav {
    visibility: collapse;
    width: 0;
    padding: 0!important;
  }
}
</style>