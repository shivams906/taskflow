import { createApp } from "vue";
import "./style.css";
import App from "./App.vue";
import router from "./router";
import { createPinia } from "pinia";
import "./assets/css/main.css";
import Toast from "vue-toastification";
import "vue-toastification/dist/index.css";
import permission from "@/directives/permission";

const app = createApp(App);
app.use(createPinia());
app.use(router);
app.directive("permission", permission);

const options = {
  position: "top-right",
  timeout: 5000,
  closeOnClick: true,
  pauseOnHover: true,
  transition: "Vue-Toastification__fade",
};

app.use(Toast, options);
app.mount("#app");
