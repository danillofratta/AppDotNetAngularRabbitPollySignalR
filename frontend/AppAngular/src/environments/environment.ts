export const environment = {
  production: true,

  // ApiUrlStock: 'http://localhost:5003',
  // ApiUrlStockSignal: 'http://localhost:5003/notificationHub',

  // ApiUrlSale: 'https://localhost:5002',
  // ApiUrlSaleSignal: 'https://localhost:5002/notificationHub',

  // ApiUrlOrder: 'https://localhost:5001',
  // ApiUrlOrderSignal: 'https://localhost:5001/notificationHub',

  // ApiUrlProduct: 'http://localhost:5003',

  // ng serve --configuration=production

  //gateway precisa alterar os endpoints das chamadas também, ver como fazer
  ApiUrlStock: 'http://localhost:4000',
  ApiUrlStockSignal: 'http://localhost:4000/stocknotificationHub',
  //ApiUrlStockSignal: '/stocknotificationHub',

  ApiUrlSale: 'http://localhost:4000',
  ApiUrlSaleSignal: 'http://localhost:4000/salenotificationHub',
  //ApiUrlSaleSignal: '/salenotificationHub',

  ApiUrlOrder: 'http://localhost:4000',
  ApiUrlOrderSignal: 'http://localhost:4000/ordernotificationHub',
  //ApiUrlOrderSignal: '/ordernotificationHub',

  ApiUrlProduct: 'http://localhost:4000',
};
