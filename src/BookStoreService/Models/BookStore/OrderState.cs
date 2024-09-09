namespace BookStoreService.Models.BookStore {
    public enum OrderState {
        CheckingOut,
        Placed,
        ReadyToShip,
        Shipped,
        Delivered,
        Cancelled
    }
}