import { DPage } from './app.po';

describe('d App', () => {
  let page: DPage;

  beforeEach(() => {
    page = new DPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
