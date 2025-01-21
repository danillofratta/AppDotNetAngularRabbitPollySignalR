export interface OrderDto{
    id: number;
    idcustomer: number;
    idproduct: number;
    nameproduct: string;
    value: number;
    createAt: Date;
    idstatus: number;
    namestatus: string;
    amount: number;
}
