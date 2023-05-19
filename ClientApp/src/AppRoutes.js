import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { AddSeries, Counter } from "./components/AddSeries";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/add-series',
    element: <AddSeries />
  },
  {
    path: '/fetch-data',
    requireAuth: true,
    element: <FetchData />
  },
  ...ApiAuthorzationRoutes
];

export default AppRoutes;
