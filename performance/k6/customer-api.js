import http from "k6/http";
import { sleep, check } from "k6";

export const options = {
  vus: 10,
  duration: "20s",
};

export default function () {
  const res = http.get("http://localhost:5000/api/customers");
  check(res, { "status is 200": (r) => r.status === 200 });
  sleep(1);
}
